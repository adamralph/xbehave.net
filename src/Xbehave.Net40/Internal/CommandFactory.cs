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

    // TODO: refactor - a beta impl if ever I saw one...
    // TODO: push test command naming into ActionCommand (and perhaps rename that type to StepCommand)
    internal class CommandFactory : ICommandFactory
    {
        private readonly IDisposer disposer;
        private int stepOrdinal;

        public CommandFactory(IDisposer disposer)
        {
            this.disposer = disposer;
        }

        public IEnumerable<ITestCommand> Create(Queue<Step> steps, MethodCall call)
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
                    var contextSuffix = (steps.Any() || isolatedContextOrdinal > 1)
                        ? " (isolated context " + (isolatedContextOrdinal++).ToString(CultureInfo.InvariantCulture) + ")"
                        : null;
                    
                    foreach (var command in this.Generate(sharedContext.Concat(step.AsEnumerable()), call, contextSuffix))
                    {
                        yield return command;
                    }
                }
                else if (!steps.Any())
                {
                    foreach (var command in this.Generate(sharedContext, call, isolatedContextOrdinal > 1 ? " (shared context)" : null))
                    {
                        yield return command;
                    }
                }
            }
        }

        private static string CreateCommandName(MethodCall call, int stepOrdinal, string stepName, string contextSuffix)
        {
            return string.Concat(
                call.Name,
                ".",
                stepOrdinal.ToString("D2", CultureInfo.InvariantCulture),
                ".",
                "\"",
                stepName,
                "\"",
                contextSuffix);
        }

        private static string CreateDisposalCommandName(MethodCall call, int stepOrdinal, string contextSuffix)
        {
            return string.Concat(
                call.Name,
                ".",
                stepOrdinal.ToString("D2", CultureInfo.InvariantCulture),
                ".",
                "Disposal",
                contextSuffix);
        }

        private IEnumerable<ITestCommand> Generate(IEnumerable<Step> steps, MethodCall call, string contextSuffix)
        {
            var disposables = new Stack<IDisposable>();
            foreach (var step in steps)
            {
                if (step.SkipReason != null)
                {
                    yield return new SkipCommand(call.Method, CreateCommandName(call, this.stepOrdinal++, step.Message, contextSuffix), step.SkipReason);
                }
                else
                {
                    yield return new ActionCommand(
                        call.Method,
                        CreateCommandName(call, this.stepOrdinal++, step.Message, contextSuffix),
                        MethodUtility.GetTimeoutParameter(call.Method),
                        () => disposables.Push(step.Execute()));
                }
            }

            if (disposables.Any(disposable => disposable != null))
            {
                yield return new ActionCommand(
                    call.Method,
                    CreateDisposalCommandName(call, this.stepOrdinal++, contextSuffix),
                    MethodUtility.GetTimeoutParameter(call.Method),
                    () => this.disposer.Dispose(disposables.Unwind()));
            }
        }
    }
}