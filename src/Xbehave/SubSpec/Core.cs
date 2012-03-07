// <copyright file="Core.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Xbehave.Fluent;
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

            public static IScenarioPrimitive Given(string message, ContextDelegate arrange)
            {
                EnsureThreadStaticInitialized();

                if (given == null)
                {
                    given = new SpecificationPrimitive<ContextDelegate>(message, arrange);
                }
                else
                {
                    exceptions.Add(() => { throw new InvalidOperationException("The scenario has more than one Given."); });
                }

                return given;
            }

            public static IScenarioPrimitive When(string message, Action action)
            {
                EnsureThreadStaticInitialized();

                if (when == null)
                {
                    when = new SpecificationPrimitive<Action>(message, action);
                }
                else
                {
                    exceptions.Add(() => { throw new InvalidOperationException("The scenario has more than one Given."); });
                }

                return when;
            }

            public static IScenarioPrimitive ThenInIsolation(string message, Action assert)
            {
                EnsureThreadStaticInitialized();

                var primitive = new SpecificationPrimitive<Action>(message, assert);
                thensInIsolation.Add(primitive);

                return primitive;
            }

            public static IScenarioPrimitive Then(string message, Action assert)
            {
                EnsureThreadStaticInitialized();

                var primitive = new SpecificationPrimitive<Action>(message, assert);
                thens.Add(primitive);

                return primitive;
            }

            public static IScenarioPrimitive ThenSkip(string message, Action assert)
            {
                EnsureThreadStaticInitialized();

                var primitive = new SpecificationPrimitive<Action>(message, assert);
                thenSkips.Add(primitive);

                return primitive;
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
                        "An exception was thrown while building tests from scenario {0}.{1}:\r\n{2}",
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

                    var thenInIsolationExecutor = new ThenInIsolationExecutor(given, when, thensInIsolation);
                    foreach (var item in thenInIsolationExecutor.Commands(name, method))
                    {
                        yield return item;
                        testsReturned++;
                    }

                    var thenExecutor = new ThenExecutor(given, when, thens);
                    foreach (var item in thenExecutor.Commands(name, method))
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
                    // TODO: work out way of passing reason from scenario
                    yield return new SkipCommand(method, name + ", " + skip.Message, "Action is ThenSkip (instead of Then or ThenInIsolation)");
                }
            }
        }

        private static class SpecificationPrimitiveExecutor
        {
            public static void Execute(SpecificationPrimitive<Action> primitive)
            {
                if (primitive.MillisecondsTimeout > 0)
                {
                    var result = primitive.Action.BeginInvoke(null, null);
                    if (!result.AsyncWaitHandle.WaitOne(primitive.MillisecondsTimeout))
                    {
                        throw new Xunit.Sdk.TimeoutException(primitive.MillisecondsTimeout);
                    }
                    else
                    {
                        primitive.Action.EndInvoke(result);
                    }
                }
                else
                {
                    primitive.Action();
                }
            }

            public static IDisposable Execute(SpecificationPrimitive<ContextDelegate> primitive)
            {
                if (primitive.MillisecondsTimeout > 0)
                {
                    var result = primitive.Action.BeginInvoke(null, null);
                    if (!result.AsyncWaitHandle.WaitOne(primitive.MillisecondsTimeout))
                    {
                        throw new Xunit.Sdk.TimeoutException(primitive.MillisecondsTimeout);
                    }
                    else
                    {
                        return primitive.Action.EndInvoke(result);
                    }
                }
                else
                {
                    return primitive.Action();
                }
            }
        }

        private class SpecificationPrimitive<T> : IScenarioPrimitive
        {
            private readonly string message;
            private readonly T action;
            private int millisecondsTimeout;

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

            public int MillisecondsTimeout
            {
                get { return this.millisecondsTimeout; }
            }

            public IScenarioPrimitive WithTimeout(int millisecondsTimeout)
            {
                this.millisecondsTimeout = millisecondsTimeout;
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

            public ThenInIsolationExecutor(SpecificationPrimitive<ContextDelegate> given, SpecificationPrimitive<Action> when, List<SpecificationPrimitive<Action>> thens)
            {
                this.thens = thens;
                this.given = given;
                this.when = when;
            }

            public IEnumerable<ITestCommand> Commands(string name, IMethodInfo method)
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

                    var testName = string.Format("{0}, {1}", name, then.Message);
                    yield return new ActionTestCommand(method, testName, MethodUtility.GetTimeoutParameter(method), test);
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

            public IEnumerable<ITestCommand> Commands(string name, IMethodInfo method)
            {
                if (!this.thens.Any())
                {
                    yield break;
                }

                var givenOrWhenThrewException = false;
                var arrangement = default(IDisposable);

                Action setupAction = () =>
                {
                    try
                    {
                        arrangement = SpecificationPrimitiveExecutor.Execute(given);

                        if (when != null)
                        {
                            SpecificationPrimitiveExecutor.Execute(when);
                        }
                    }
                    catch (Exception)
                    {
                        givenOrWhenThrewException = true;
                        throw;
                    }
                };

                yield return new ActionTestCommand(method, "{ " + name, 0, setupAction);

                foreach (var then in this.thens)
                {
                    // do not capture the iteration variable because 
                    // all tests would point to the same observation
                    var capturableObservation = then;
                    Action perform = () =>
                    {
                        if (givenOrWhenThrewException)
                        {
                            throw new GivenOrWhenFailedException("Execution of Given or When failed.");
                        }

                        SpecificationPrimitiveExecutor.Execute(capturableObservation);
                    };

                    yield return new ActionTestCommand(method, "\t- " + capturableObservation.Message, 0, perform);
                }

                Action disposal = () =>
                {
                    if (arrangement != null)
                    {
                        arrangement.Dispose();
                    }

                    if (givenOrWhenThrewException)
                    {
                        throw new GivenOrWhenFailedException("Execution of Given or When failed but arrangement was disposed.");
                    }
                };

                yield return new ActionTestCommand(method, "} " + name, 0, disposal);
            }
        }

        [Serializable]
        private class GivenOrWhenFailedException : Exception
        {
            public GivenOrWhenFailedException(string message)
                : base(message)
            {
            }
        }
    }
}