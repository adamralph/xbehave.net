// <copyright file="ScenarioContext.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Xbehave.Fluent;
    using Xunit.Sdk;

    internal static class ScenarioContext
    {
        [ThreadStatic]
        private static bool threadStaticInitialized;

        [ThreadStatic]
        private static DisposableStep given;

        [ThreadStatic]
        private static Step when;

        [ThreadStatic]
        private static List<Step> thensInIsolation;

        [ThreadStatic]
        private static List<Step> thens;

        [ThreadStatic]
        private static List<Step> thenSkips;

        [ThreadStatic]
        private static List<Action> throwActions;

        public static IStep Given(string message, Func<IDisposable> arrange)
        {
            EnsureThreadStaticInitialized();
            
            if (given == null)
            {
                given = new DisposableStep(message, arrange);
            }
            else
            {
                throwActions.Add(() => { throw new InvalidOperationException("The scenario has more than one Given step."); });
            }

            return given;
        }

        public static IStep When(string message, Action action)
        {
            EnsureThreadStaticInitialized();
            
            if (when == null)
            {
                when = new Step(message, action);
            }
            else
            {
                throwActions.Add(() => { throw new InvalidOperationException("The scenario has more than one When step."); });
            }

            return when;
        }

        public static IStep ThenInIsolation(string message, Action assert)
        {
            EnsureThreadStaticInitialized();
            
            var step = new Step(message, assert);
            thensInIsolation.Add(step);
            return step;
        }

        public static IStep Then(string message, Action assert)
        {
            EnsureThreadStaticInitialized();
            
            var step = new Step(message, assert);
            thens.Add(step);
            return step;
        }

        public static IStep ThenSkip(string message, Action assert)
        {
            EnsureThreadStaticInitialized();
            
            var step = new Step(message, assert);
            thenSkips.Add(step);
            return step;
        }

        // TODO: address DoNotCatchGeneralExceptionTypes
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Part of the original SubSpec code - will be addressed.")]
        public static IEnumerable<ITestCommand> GetTestCommands(IMethodInfo method, Action registerSteps)
        {
            try
            {
                registerSteps();
                return BuildCommandsFromRegisteredSteps(method);
            }
            catch (Exception ex)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "An exception was thrown while building tests from scenario {0}.{1}:\r\n{2}",
                    method.TypeName,
                    method.Name,
                    ex.ToString());

                return new ITestCommand[] { new ExceptionTestCommand(method, () => { throw new InvalidOperationException(message); }) };
            }
        }

        private static void Reset()
        {
            throwActions = new List<Action>();
            given = null;
            when = null;
            thensInIsolation = new List<Step>();
            thens = new List<Step>();
            thenSkips = new List<Step>();
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

        private static IEnumerable<ITestCommand> BuildCommandsFromRegisteredSteps(IMethodInfo method)
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
                        () => { throw new InvalidOperationException("The scenario does not have at least one Then step."); });
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
                throwActions.Add(() => { throw new InvalidOperationException("The scenario has no Given step."); });
            }

            if (throwActions.Count > 0)
            {
                // throw the first recorded exception, preserves stacktraces nicely.
                return new ExceptionTestCommand(method, () => throwActions[0]());
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
}