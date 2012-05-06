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
        public IEnumerable<ITestCommand> Create(Queue<Step> steps, MethodCall call)
        {
            var sharedContext = new Queue<Step>();            
            var isolatedContextOrdinal = 1;
            foreach (var step in steps.DequeueAll())
            {
                if (!step.InIsolation)
                {
                    sharedContext.Enqueue(step);
                }

                if (step.InIsolation)
                {
                    var context = (steps.Any() || isolatedContextOrdinal > 1)
                        ? "context " + (isolatedContextOrdinal++).ToString(CultureInfo.InvariantCulture)
                        : null;

                    foreach (var command in Generate(sharedContext.Concat(step.AsEnumerable()), call, context))
                    {
                        yield return command;
                    }
                }
                else if (!steps.Any())
                {
                    var context = isolatedContextOrdinal > 1 ? "shared context" : null;
                    foreach (var command in Generate(sharedContext, call, context))
                    {
                        yield return command;
                    }
                }
            }
        }

        private static IEnumerable<ITestCommand> Generate(IEnumerable<Step> steps, MethodCall call, string context)
        {
            var stepOrdinal = 1;
            var disposables = new Stack<IDisposable>();
            foreach (var step in steps)
            {
                yield return new StepCommand(call, stepOrdinal++, step, context, result => disposables.Push(result));
            }

            if (disposables.Any(disposable => disposable != null))
            {
                yield return new DisposalCommand(call, stepOrdinal++, context, disposables.Unwind());
            }
        }
    }
}