// <copyright file="StepCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Xbehave.Sdk.Infrastructure;
    using Xunit.Sdk;
    using Guard = Xbehave.Sdk.Infrastructure.Guard;

    public class StepCommand : TestCommand
    {
        private readonly string name;
        private readonly Step step;

        public StepCommand(IMethodInfo method, IEnumerable<object> args, string scenarioName, int contextOrdinal, int stepOrdinal, Step step)
            : base(method, string.Empty, method.GetTimeoutParameter())
        {
            Guard.AgainstNullArgument("args", args);
            Guard.AgainstNullArgument("step", step);

            var provider = CultureInfo.InvariantCulture;
            var stepName = step.Name.AttemptFormatInvariantCulture(args.ToArray());
            this.name = string.Format(provider, "[{0}.{1}] {2}", contextOrdinal.ToString("D2", provider), stepOrdinal.ToString("D2", provider), stepName);
            this.DisplayName = string.Concat(scenarioName, " ", this.name);
            this.step = step;
        }

        public override MethodResult Execute(object testClass)
        {
            if (this.step.SkipReason != null)
            {
                return new SkipResult(this.testMethod, this.DisplayName, this.step.SkipReason);
            }

            if (Context.FailedStepName != null)
            {
                var message = string.Format(CultureInfo.InvariantCulture, "Failed to execute preceding step \"{0}\".", Context.FailedStepName);
                throw new InvalidOperationException(message);
            }

            try
            {
                this.step.Execute();
            }
            catch (Exception)
            {
                Context.FailedStepName = this.name;
                throw;
            }

            return new PassedResult(this.testMethod, this.DisplayName);
        }
    }
}