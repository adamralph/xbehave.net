// <copyright file="CurrentScenario.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Xunit.Extensions;
    using Xunit.Sdk;

    public static class CurrentScenario
    {
        [ThreadStatic]
        private static bool addingBackgroundSteps;

        [ThreadStatic]
        private static List<Step> steps;

        [ThreadStatic]
        private static List<Action> teardowns;

        private static List<Step> Steps
        {
            get { return steps ?? (steps = new List<Step>()); }
        }

        private static List<Action> Teardowns
        {
            get { return teardowns ?? (teardowns = new List<Action>()); }
        }

        public static Step AddStep(string name, Action body)
        {
            var step = addingBackgroundSteps ? new BackgroundStep(name, body) : new Step(name, body);
            Steps.Add(step);
            return step;
        }

        public static void AddTeardown(Action teardown)
        {
            if (teardown != null)
            {
                Teardowns.Add(teardown);
            }
        }

        public static IEnumerable<Action> ExtractTeardowns()
        {
            try
            {
                return Teardowns;
            }
            finally
            {
                teardowns = null;
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Required to prevent infinite loops in test runners (TestDrive.NET, Resharper) when they are allowed to handle exceptions.")]
        public static IEnumerable<ITestCommand> ExtractCommands(
            IMethodInfo method,
            IEnumerable<ITestCommand> backgroundCommands,
            ITestCommand scenarioCommand)
        {
            Guard.AgainstNullArgument("backgroundCommands", backgroundCommands);
            Guard.AgainstNullArgument("scenarioCommand", scenarioCommand);

            try
            {
                try
                {
                    var feature = method.IsAbstract ? null : method.CreateInstance();

                    addingBackgroundSteps = true;
                    foreach (var command in backgroundCommands)
                    {
                        command.Execute(feature);
                    }

                    addingBackgroundSteps = false;
                    scenarioCommand.Execute(feature);
                }
                catch (Exception ex)
                {
                    return new ITestCommand[] { new ExceptionCommand(method, ex) };
                }

                var theoryCommand = scenarioCommand as TheoryCommand;
                var args = theoryCommand == null ? new object[0] : theoryCommand.Parameters;
                var contexts = new ContextFactory().CreateContexts(method, args, Steps).ToArray();
                return contexts.SelectMany((context, index) => context.CreateTestCommands(index + 1));
            }
            finally
            {
                steps = null;
            }
        }
    }
}