// <copyright file="StepCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Globalization;
    using Xbehave.Sdk.Infrastructure;
    using Xunit.Extensions;
    using Xunit.Sdk;
    using Guard = Xbehave.Sdk.Infrastructure.Guard;

    [CLSCompliant(false)]
    public class StepCommand : TheoryCommand
    {
        private readonly string name;
        private readonly Step step;

        public StepCommand(IMethodInfo method, Type[] genericTypes, object[] args, int contextOrdinal, int stepOrdinal, Step step)
            : base(method, args, genericTypes)
        {
            Guard.AgainstNullArgument("step", step);
            Guard.AgainstNullArgumentProperty("step", "Name", step.Name);

            this.step = step;

            var provider = CultureInfo.InvariantCulture;
            string stepName;
            try
            {
                stepName = string.Format(provider, step.Name, args);
            }
            catch (FormatException)
            {
                stepName = step.Name;
            }

            this.name = string.Format(provider, "[{0}.{1}] {2}", contextOrdinal.ToString("D2", provider), stepOrdinal.ToString("D2", provider), stepName);
            this.DisplayName = string.Format(CultureInfo.InvariantCulture, "{0} {1}", this.DisplayName, this.name);
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
