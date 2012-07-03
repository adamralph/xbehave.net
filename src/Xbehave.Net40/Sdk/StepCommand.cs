// <copyright file="StepCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Globalization;
    using Xbehave.Infra;
    using Xunit.Sdk;

    internal class StepCommand : CommandBase
    {
        private readonly Step step;

        public StepCommand(ScenarioDefinition definition, int contextOrdinal, int ordinal, Step step)
            : base(definition, contextOrdinal, ordinal, step.Name.AttemptFormatInvariantCulture(definition.Args))
        {
            this.step = step;
        }

        public override MethodResult Execute(object testClass)
        {
            if (this.step.SkipReason != null)
            {
                return new SkipResult(this.testMethod, this.DisplayName, this.step.SkipReason);
            }

            if (Context.FailedStepCommandName != null)
            {
                var reason = string.Format(CultureInfo.InvariantCulture, "Skipped due to failure of previous step \"{0}\".", Context.FailedStepCommandName);
                return new SkipResult(this.testMethod, this.DisplayName, reason);
            }

            try
            {
                this.step.Execute();
            }
            catch (Exception)
            {
                Context.FailedStepCommandName = this.LocalName;
                throw;
            }

            return new PassedResult(this.testMethod, this.DisplayName);
        }
    }
}