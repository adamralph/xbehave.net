// <copyright file="StepCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Globalization;
    using Xunit.Sdk;

    [CLSCompliant(false)]
    public class StepCommand : ContextCommand
    {
        private readonly Step step;

        public StepCommand(IMethodInfo method, object[] arguments, Type[] typeArguments, int contextOrdinal, int stepOrdinal, Step step)
            : base(method, arguments, typeArguments, contextOrdinal, stepOrdinal)
        {
            Guard.AgainstNullArgument("step", step);
            Guard.AgainstNullArgumentProperty("step", "Name", step.Name);

            this.step = step;

            var provider = CultureInfo.InvariantCulture;
            string stepName;
            try
            {
                stepName = string.Format(provider, step.Name, arguments);
            }
            catch (FormatException)
            {
                stepName = step.Name;
            }

            this.Name = string.Format(provider, "{0} {1}", this.Name, stepName);
            this.DisplayName = string.Format(CultureInfo.InvariantCulture, "{0} {1}", this.DisplayName, stepName);
        }

        public override MethodResult Execute(object testClass)
        {
            if (this.step.SkipReason != null)
            {
                return new SkipResult(this.testMethod, this.DisplayName, this.step.SkipReason);
            }

            if (Context.FailedStepName != null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Failed to execute preceding step \"{0}\".", Context.FailedStepName));
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
