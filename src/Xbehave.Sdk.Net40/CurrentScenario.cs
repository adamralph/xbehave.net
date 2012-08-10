// <copyright file="CurrentScenario.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Xunit.Sdk;
    using Guard = Xbehave.Sdk.Infrastructure.Guard;

    public static class CurrentScenario
    {
        [ThreadStatic]
        private static bool addingBackgroundSteps;

        [ThreadStatic]
        private static List<Step> steps;

        [ThreadStatic]
        private static List<IDisposable> disposables;

        private static List<Step> Steps
        {
            get { return steps ?? (steps = new List<Step>()); }
        }

        private static List<IDisposable> Disposables
        {
            get { return disposables ?? (disposables = new List<IDisposable>()); }
        }

        public static Step AddStep(string name, Action body)
        {
            var step = addingBackgroundSteps ? new BackgroundStep(name, body) : new Step(name, body);
            Steps.Add(step);
            return step;
        }

        public static void AddDisposable(IDisposable disposable)
        {
            if (disposable != null)
            {
                Disposables.Add(disposable);
            }
        }

        public static IEnumerable<IDisposable> ExtractDisposables()
        {
            try
            {
                return Disposables;
            }
            finally
            {
                disposables = null;
            }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Required to prevent infinite loops in test runners (TestDrive.NET, Resharper) when they are allowed to handle exceptions.")]
        public static IEnumerable<ITestCommand> ExtractCommands(
            IMethodInfo method,
            IEnumerable<Type> genericTypes,
            IEnumerable<object> args,
            IEnumerable<ITestCommand> backgroundCommands,
            ITestCommand scenarioCommand,
            object feature)
        {
            Guard.AgainstNullArgument("backgroundCommands", backgroundCommands);
            Guard.AgainstNullArgument("scenarioCommand", scenarioCommand);

            try
            {
                try
                {
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

                var contexts = new ContextFactory().CreateContexts(method, genericTypes, args, Steps).ToArray();
                return contexts.SelectMany((context, index) => context.CreateTestCommands(index + 1));
            }
            finally
            {
                steps = null;
            }
        }
    }
}