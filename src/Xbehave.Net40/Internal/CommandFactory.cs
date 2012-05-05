// <copyright file="CommandFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Xbehave.Infra;
    using Xunit.Sdk;

    internal class CommandFactory : ICommandFactory
    {
        private readonly IDisposer disposer;
        private int stepOrdinal;

        public CommandFactory(IDisposer disposer)
        {
            this.disposer = disposer;
        }

        public IEnumerable<ITestCommand> Create(Queue<Step> steps, IMethodInfo method)
        {
            var sharedContext = new Queue<Step>();
            this.stepOrdinal = 1;
            var isolatedContextOrdinal = 1;
            foreach (var step in steps.DequeueAll())
            {
                if (!step.InIsolation)
                {
                    sharedContext.Enqueue(step);
                }

                if (step.InIsolation)
                {
                    var contextSuffix = steps.Any() ? " (isolated context " + (isolatedContextOrdinal++).ToString(CultureInfo.InvariantCulture) + ")" : null;
                    foreach (var command in this.Generate(sharedContext.Concat(step.AsEnumerable()), method, contextSuffix))
                    {
                        yield return command;
                    }
                }
                else if (!steps.Any())
                {
                    foreach (var command in this.Generate(sharedContext.Concat(step.AsEnumerable()), method, isolatedContextOrdinal > 1 ? " (shared context)" : null))
                    {
                        yield return command;
                    }
                }
            }
        }

        private static string CreateCommandName(string scenarioName, int stepOrdinal, string stepName, string contextSuffix)
        {
            return string.Concat(
                scenarioName,
                ".",
                stepOrdinal.ToString("D2", CultureInfo.InvariantCulture),
                ".",
                "\"",
                stepName,
                "\"",
                contextSuffix);
        }

        private static string CreateDisposalCommandName(string scenarioName, int stepOrdinal, string contextSuffix)
        {
            return string.Concat(
                scenarioName,
                ".",
                stepOrdinal.ToString("D2", CultureInfo.InvariantCulture),
                ".",
                "Disposal",
                contextSuffix);
        }

        private static void ThrowIfBadStep(Step badStep, int badStepOrdinal)
        {
            if (badStep != null)
            {
                var message = string.Format(CultureInfo.InvariantCulture, "Execution of {0}.\"{1}\" failed.", badStepOrdinal.ToString("D2"), badStep.Message);
                throw new InvalidOperationException(message);
            }
        }

        private IEnumerable<ITestCommand> Generate(IEnumerable<Step> steps, IMethodInfo method, string contextSuffix)
        {
            var disposables = new Stack<IDisposable>();
            foreach (var step in steps)
            {
                if (step.SkipReason != null)
                {
                    yield return new SkipCommand(method, CreateCommandName(method.Name, this.stepOrdinal++, step.Message, contextSuffix), step.SkipReason);
                }
                else
                {
                    yield return new ActionCommand(
                        method,
                        CreateCommandName(method.Name, this.stepOrdinal++, step.Message, contextSuffix),
                        MethodUtility.GetTimeoutParameter(method),
                        () => disposables.Push(step.Execute()));
                }
            }

            if (disposables.Any(disposable => disposable != null))
            {
                yield return new ActionCommand(
                    method,
                    CreateDisposalCommandName(method.Name, this.stepOrdinal++, contextSuffix),
                    MethodUtility.GetTimeoutParameter(method),
                    () => this.disposer.Dispose(disposables.Unwind()));
            }
        }
    }
}