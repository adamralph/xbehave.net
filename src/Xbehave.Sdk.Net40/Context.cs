// <copyright file="Context.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xbehave.Sdk.Infrastructure;

    public class Context
    {
        [ThreadStatic]
        private static string failedStepName;

        private readonly IMethodInfo method;
        private readonly object[] args;
        private readonly IEnumerable<Step> steps;

        public Context(IMethodInfo method, IEnumerable<object> args, IEnumerable<Step> steps)
        {
            Guard.AgainstNullArgument("args", args);
            Guard.AgainstNullArgument("steps", steps);

            this.method = method;
            this.args = args.ToArray();
            this.steps = steps;
        }

        public static string FailedStepName
        {
            get { return failedStepName; }
            set { failedStepName = value; }
        }

        public IEnumerable<ITestCommand> CreateTestCommands(int contextOrdinal)
        {
            FailedStepName = null;
            var stepOrdinal = 1;
            foreach (var step in this.steps)
            {
                yield return new StepCommand(this.method, this.args, contextOrdinal, stepOrdinal++, step);
            }

            // NOTE: this relies on the test runner executing each above yielded step command and below yielded disposal command as soon as it is recieved
            // TD.NET, R# and xunit.console all seem to do this
            FailedStepName = null;
            var index = 0;
            while (true)
            {
                var disposables = CurrentScenario.ExtractDisposables();
                if (!disposables.Any())
                {
                    break;
                }

                // don't reverse odd disposables since their creation order has already been reversed by the previous command
                var disposalStep = new DisposalStep(index % 2 == 0 ? disposables.Reverse() : disposables);
                yield return new StepCommand(this.method, this.args, contextOrdinal, stepOrdinal++, disposalStep);

                ++index;
            }
        }
    }
}