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

    internal static class CurrentScenario
    {
        [ThreadStatic]
        private static bool addingBackgroundSteps;

        [ThreadStatic]
        private static List<Step> steps;

        [ThreadStatic]
        private static List<IDisposable> disposables;

        public static bool AddingBackgroundSteps
        {
            get { return addingBackgroundSteps; }
            set { addingBackgroundSteps = value; }
        }

        private static List<Step> Steps
        {
            get { return steps ?? (steps = new List<Step>()); }
        }

        private static List<IDisposable> Disposables
        {
            get { return disposables ?? (disposables = new List<IDisposable>()); }
        }

        public static Step AddStep(string stepType, string text, Action body)
        {
            var step = new Step(stepType, text, addingBackgroundSteps, body);
            Steps.Add(step);
            return step;
        }

        public static void AddDisposable(IDisposable disposable)
        {
            if (disposable == null)
            {
                return;
            }

            Disposables.Add(disposable);
        }

        public static IEnumerable<IDisposable> GetDisposables()
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
        public static IEnumerable<ITestCommand> CreateCommands(ScenarioDefinition definition)
        {
            try
            {
                try
                {
                    definition.Execute();
                }
                catch (Exception ex)
                {
                    return new ITestCommand[] { new ExceptionCommand(definition.Method, ex) };
                }

                var contexts = new ContextFactory().CreateContexts(definition, Steps).ToArray();
                return contexts.SelectMany((context, index) => context.CreateTestCommands(index + 1));
            }
            finally
            {
                steps = null;
            }
        }
    }
}