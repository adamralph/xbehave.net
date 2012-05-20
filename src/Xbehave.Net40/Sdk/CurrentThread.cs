// <copyright file="CurrentThread.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Xunit.Sdk;

    internal static class CurrentThread
    {
        [ThreadStatic]
        private static Scenario scenario;

        public static Scenario Scenario
        {
            get { return scenario ?? (scenario = new Scenario()); }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Required to prevent infinite loops in test runners (TestDrive.NET, Resharper) when they are allowed to handle exceptions.")]
        public static IEnumerable<ITestCommand> CreateCommands(ScenarioDefinition definition)
        {
            // NOTE: I've tried to move this into Scenario, with the finally block clearing the steps but it just doesn't seem to work
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

                var contexts = Scenario.CreateContexts(definition).ToArray();
                return contexts.SelectMany((context, index) => context.CreateTestCommands(contexts.Length > 1 ? (int?)(index + 1) : null));
            }
            finally
            {
                scenario = null;
            }
        }
    }
}