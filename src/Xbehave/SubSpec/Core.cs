// <copyright file="Core.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Xunit.Sdk;

    /// <summary>
    /// The core SubSpec implementation.
    /// </summary>
    internal static class Core
    {
        internal static class SpecificationContext
        {
            [ThreadStatic]
            private static bool threadStaticInitialized;

            [ThreadStatic]
            private static SpecificationPrimitive<ContextDelegate> given;

            [ThreadStatic]
            private static SpecificationPrimitive<Action> when;

            [ThreadStatic]
            private static List<SpecificationPrimitive<Action>> thensInIsolation;

            [ThreadStatic]
            private static List<SpecificationPrimitive<Action>> thens;

            [ThreadStatic]
            private static List<SpecificationPrimitive<Action>> thenSkips;

            [ThreadStatic]
            private static List<Action> exceptions;

            public static ISpecificationPrimitive Context(string message, ContextDelegate arrange)
            {
                EnsureThreadStaticInitialized();

                if (given == null)
                {
                    given = new SpecificationPrimitive<ContextDelegate>(message, arrange);
                }
                else
                {
                    exceptions.Add(() => { throw new InvalidOperationException("Cannot have more than one Context statement in a specification"); });
                }

                return given;
            }

            public static ISpecificationPrimitive Do(string message, Action doAction)
            {
                EnsureThreadStaticInitialized();

                if (when == null)
                {
                    when = new SpecificationPrimitive<Action>(message, doAction);
                }
                else
                {
                    exceptions.Add(() => { throw new InvalidOperationException("The scenario has more than one Given."); });
                }

                return when;
            }

            public static ISpecificationPrimitive Assert(string message, Action assertAction)
            {
                EnsureThreadStaticInitialized();

                SpecificationPrimitive<Action> assert = new SpecificationPrimitive<Action>(message, assertAction);
                thensInIsolation.Add(assert);

                return assert;
            }

            public static ISpecificationPrimitive Observation(string message, Action observationAction)
            {
                EnsureThreadStaticInitialized();

                var observation = new SpecificationPrimitive<Action>(message, observationAction);
                thens.Add(observation);

                return observation;
            }

            public static ISpecificationPrimitive Todo(string message, Action skippedAction)
            {
                EnsureThreadStaticInitialized();

                SpecificationPrimitive<Action> skip = new SpecificationPrimitive<Action>(message, skippedAction);
                thenSkips.Add(skip);

                return skip;
            }

            public static IEnumerable<ITestCommand> SafelyEnumerateTestCommands(IMethodInfo method, Action<IMethodInfo> registerPrimitives)
            {
                try
                {
                    registerPrimitives(method);
                    return SpecificationContext.BuildCommandsFromRegisteredPrimitives(method);
                }
                catch (Exception ex)
                {
                    var message = string.Format(
                        "An exception was thrown while building tests from Specification {0}.{1}:\r\n{2}",
                        method.TypeName,
                        method.Name,
                        ex.ToString());

                    return new ITestCommand[] { new ExceptionTestCommand(method, () => { throw new InvalidOperationException(message); }) };
                }
            }

            private static void Reset()
            {
                exceptions = new List<Action>();
                given = null;
                when = null;
                thensInIsolation = new List<SpecificationPrimitive<Action>>();
                thens = new List<SpecificationPrimitive<Action>>();
                thenSkips = new List<SpecificationPrimitive<Action>>();
            }

            private static void EnsureThreadStaticInitialized()
            {
                if (threadStaticInitialized)
                {
                    return;
                }

                Reset();
                threadStaticInitialized = true;
            }

            private static string PrepareSetupDescription()
            {
                return when == null
                    ? given.Message
                    : string.Concat(given.Message, " ", when.Message);
            }

            private static IEnumerable<ITestCommand> BuildCommandsFromRegisteredPrimitives(IMethodInfo method)
            {
                EnsureThreadStaticInitialized();

                try
                {
                    var validationException = ValidateSpecification(method);
                    if (validationException != null)
                    {
                        yield return validationException;
                        yield break;
                    }

                    int testsReturned = 0;
                    string name = PrepareSetupDescription();

                    var ax = new ThenInIsolationExecutor(given, when, thensInIsolation);
                    foreach (var item in ax.AssertCommands(name, method))
                    {
                        yield return item;
                        testsReturned++;
                    }

                    var ox = new ThenExecutor(given, when, thens);
                    foreach (var item in ox.ObservationCommands(name, method))
                    {
                        yield return item;
                        testsReturned++;
                    }

                    foreach (var item in SkipCommands(name, method))
                    {
                        yield return item;
                        testsReturned++;
                    }

                    if (testsReturned == 0)
                    {
                        yield return new ExceptionTestCommand(
                            method,
                            () => { throw new InvalidOperationException("The scenario does not have at least one Then."); });
                    }
                }
                finally
                {
                    Reset();
                }
            }

            private static ExceptionTestCommand ValidateSpecification(IMethodInfo method)
            {
                if (given == null)
                {
                    exceptions.Add(() => { throw new InvalidOperationException("The scenario has no Given."); });
                }

                if (exceptions.Count > 0)
                {
                    // throw the first recorded exception, preserves stacktraces nicely.
                    return new ExceptionTestCommand(method, () => exceptions[0]());
                }

                return null;
            }

            private static IEnumerable<ITestCommand> SkipCommands(string name, IMethodInfo method)
            {
                foreach (var skip in thenSkips)
                {
                    yield return new SkipCommand(method, name + ", " + skip.Message, "Action is ThenSkip (instead of Then or ThenInIsolation)");
                }
            }
        }

        private static class SpecificationPrimitiveExecutor
        {
            public static void Execute(SpecificationPrimitive<Action> primitive)
            {
                if (primitive.TimeoutMs > 0)
                {
                    IAsyncResult asyncResult = primitive.Action.BeginInvoke(null, null);
                    if (!asyncResult.AsyncWaitHandle.WaitOne(primitive.TimeoutMs))
                    {
                        throw new Xunit.Sdk.TimeoutException(primitive.TimeoutMs);
                    }
                    else
                    {
                        primitive.Action.EndInvoke(asyncResult);
                    }
                }
                else
                {
                    primitive.Action();
                }
            }

            public static IDisposable Execute(SpecificationPrimitive<ContextDelegate> primitive)
            {
                if (primitive.TimeoutMs > 0)
                {
                    IAsyncResult asyncResult = primitive.Action.BeginInvoke(null, null);

                    if (!asyncResult.AsyncWaitHandle.WaitOne(primitive.TimeoutMs))
                    {
                        throw new Xunit.Sdk.TimeoutException(primitive.TimeoutMs);
                    }
                    else
                    {
                        return primitive.Action.EndInvoke(asyncResult);
                    }
                }
                else
                {
                    return primitive.Action();
                }
            }
        }

        private class SpecificationPrimitive<T> : ISpecificationPrimitive
        {
            private readonly string message;
            private readonly T action;
            private int timeoutMs;

            public SpecificationPrimitive(string message, T action)
            {
                if (message == null)
                {
                    throw new ArgumentNullException("message");
                }
                
                if (action == null)
                {
                    throw new ArgumentNullException("action");
                }

                this.message = message;
                this.action = action;
            }

            public string Message
            {
                get { return this.message; }
            }

            public T Action
            {
                get { return this.action; }
            }

            public int TimeoutMs
            {
                get { return this.timeoutMs; }
            }

            public ISpecificationPrimitive WithTimeout(int timeoutMs)
            {
                this.timeoutMs = timeoutMs;
                return this;
            }
        }

        private class ActionTestCommand : TestCommand, ITestCommand
        {
            private readonly Action action;

            public ActionTestCommand(IMethodInfo method, string name, int timeout, Action action)
                : base(method, name, timeout)
            {
                this.action = action;
            }

            public override MethodResult Execute(object testClass)
            {
                try
                {
                    this.action();
                    return new PassedResult(testMethod, DisplayName);
                }
                catch (Exception ex)
                {
                    return new FailedResult(testMethod, ex, DisplayName);
                }
            }
        }

        private class ExceptionTestCommand : ActionTestCommand
        {
            public ExceptionTestCommand(IMethodInfo method, Xunit.Assert.ThrowsDelegate action)
                : base(method, null, 0, () => action())
            {
            }

            public override bool ShouldCreateInstance
            {
                get { return false; }
            }
        }

        private class ThenInIsolationExecutor
        {
            private readonly SpecificationPrimitive<ContextDelegate> given;
            private readonly SpecificationPrimitive<Action> when;
            private readonly List<SpecificationPrimitive<Action>> thens;

            public ThenInIsolationExecutor(
                SpecificationPrimitive<ContextDelegate> given, SpecificationPrimitive<Action> when, List<SpecificationPrimitive<Action>> thens)
            {
                this.thens = thens;
                this.given = given;
                this.when = when;
            }

            public IEnumerable<ITestCommand> AssertCommands(string name, IMethodInfo method)
            {
                foreach (var then in this.thens)
                {
                    // do not capture the iteration variable because 
                    // all tests would point to the same assertion
                    var capturableAssertion = then;
                    Action test = () =>
                    {
                        using (SpecificationPrimitiveExecutor.Execute(given))
                        {
                            if (this.when != null)
                            {
                                SpecificationPrimitiveExecutor.Execute(when);
                            }

                            SpecificationPrimitiveExecutor.Execute(capturableAssertion);
                        }
                    };

                    var testDescription = string.Format("{0}, {1}", name, then.Message);
                    yield return new ActionTestCommand(method, testDescription, MethodUtility.GetTimeoutParameter(method), test);
                }
            }
        }

        private class ThenExecutor
        {
            private readonly SpecificationPrimitive<ContextDelegate> given;
            private readonly SpecificationPrimitive<Action> when;
            private readonly IEnumerable<SpecificationPrimitive<Action>> thens;

            public ThenExecutor(SpecificationPrimitive<ContextDelegate> given, SpecificationPrimitive<Action> when, IEnumerable<SpecificationPrimitive<Action>> thens)
            {
                this.thens = thens;
                this.given = given;
                this.when = when;
            }

            public IEnumerable<ITestCommand> ObservationCommands(string name, IMethodInfo method)
            {
                if (!this.thens.Any())
                {
                    yield break;
                }

                var setupExceptionOccurred = false;
                var systemUnderTest = default(IDisposable);

                Action setupAction = () =>
                {
                    try
                    {
                        systemUnderTest = SpecificationPrimitiveExecutor.Execute(given);

                        if (when != null)
                        {
                            SpecificationPrimitiveExecutor.Execute(when);
                        }
                    }
                    catch (Exception)
                    {
                        setupExceptionOccurred = true;
                        throw;
                    }
                };

                yield return new ActionTestCommand(method, "{ " + name, 0, setupAction);

                foreach (var observation in this.thens)
                {
                    // do not capture the iteration variable because 
                    // all tests would point to the same observation
                    var capturableObservation = observation;
                    Action perform = () =>
                    {
                        if (setupExceptionOccurred)
                        {
                            throw new ContextSetupFailedException("Setting up Context failed");
                        }

                        SpecificationPrimitiveExecutor.Execute(capturableObservation);
                    };

                    yield return new ActionTestCommand(method, "\t- " + capturableObservation.Message, 0, perform);
                }

                Action tearDownAction = () =>
                {
                    if (systemUnderTest != null)
                    {
                        systemUnderTest.Dispose();
                    }

                    if (setupExceptionOccurred)
                    {
                        throw new ContextSetupFailedException("Setting up Context failed, but Fixtures were disposed.");
                    }
                };

                yield return new ActionTestCommand(method, "} " + name, 0, tearDownAction);
            }
        }

        /// <summary>
        /// An exception that is thrown from Observations or their teardown whenever the corresponding setup failed.
        /// </summary>
        private class ContextSetupFailedException : Exception
        {
            public ContextSetupFailedException(string message)
                : base(message)
            {
            }
        }
    }
}