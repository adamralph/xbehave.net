// <copyright file="StepCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Globalization;
    using Xunit.Sdk;
    using Guard = Xbehave.Sdk.Infrastructure.Guard;

    public class StepCommand : CommandBase
    {
        private readonly Step step;

        // TODO: stepName and step seems wrong - reconsider
        public StepCommand(IMethodInfo method, string scenarioName, int contextOrdinal, int stepOrdinal, string stepName, Step step)
            : base(method, scenarioName, contextOrdinal, stepOrdinal, stepName)
        {
            Guard.AgainstNullArgument("step", step);

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
                Context.FailedStepName = this.Name;
                throw;
            }

            return new PassedResult(this.testMethod, this.DisplayName);
        }
    }
}