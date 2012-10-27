// <copyright file="Context.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    public class Context
    {
        [ThreadStatic]
        private static string failedStepName;

        private readonly IMethodInfo method;
        private readonly Argument[] arguments;
        private readonly Type[] typeArguments;
        private readonly Step[] steps;

        public Context(IMethodInfo method, IEnumerable<Argument> arguments, IEnumerable<Type> typeArguments, IEnumerable<Step> steps)
        {
            Guard.AgainstNullArgument("arguments", arguments);
            Guard.AgainstNullArgument("typeArguments", typeArguments);
            Guard.AgainstNullArgument("steps", steps);

            this.method = method;
            this.arguments = arguments.ToArray();
            this.typeArguments = typeArguments.ToArray();
            this.steps = steps.ToArray();
        }

        public static string FailedStepName
        {
            get { return failedStepName; }
            set { failedStepName = value; }
        }

        public IEnumerable<ITestCommand> CreateCommands(int contextOrdinal)
        {
            FailedStepName = null;
            var stepOrdinal = 1;
            foreach (var step in this.steps)
            {
                yield return new StepCommand(this.method, this.arguments, this.typeArguments, contextOrdinal, stepOrdinal++, step);
            }

            FailedStepName = null;

            // NOTE: this relies on the test runner executing each above yielded step command and below yielded disposal command as soon as it is recieved
            // TD.NET, R# and xunit.console all seem to do this
            var odd = true;
            while (true)
            {
                var teardowns = CurrentScenario.ExtractTeardowns().ToArray();
                if (!teardowns.Any())
                {
                    break;
                }

                // don't reverse even disposables since their creation order has already been reversed by the previous command
                yield return new TeardownCommand(this.method, this.arguments, this.typeArguments, contextOrdinal, stepOrdinal++, odd ? teardowns.Reverse() : teardowns);
                odd = !odd;
            }
        }
    }
}