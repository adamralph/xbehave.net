// <copyright file="StepCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Globalization;
    using Xunit.Sdk;
    using Guard = Xbehave.Sdk.Infrastructure.Guard;

    public class StepCommand : TestCommand
    {
        private readonly string name;
        private readonly Step step;

        // TODO: stepName and step seems wrong - address
        public StepCommand(IMethodInfo method, string scenarioName, int contextOrdinal, int stepOrdinal, string stepName, Step step)
            : base(method, stepName, method.GetTimeoutParameter())
        {
            Guard.AgainstNullArgument("step", step);

            var provider = CultureInfo.InvariantCulture;
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