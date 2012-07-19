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

        public StepCommand(IMethodInfo method, IEnumerable<object> args, int contextOrdinal, int stepOrdinal, Step step)
            : base(method, string.Empty, method.GetTimeoutParameter())
        {
            Guard.AgainstNullArgument("method", method);
            Guard.AgainstNullArgument("args", args);
            Guard.AgainstNullArgument("step", step);

            this.step = step;

            var provider = CultureInfo.InvariantCulture;
            var stepName = step.Name.AttemptFormatInvariantCulture(args.ToArray());
            this.name = string.Format(provider, "[{0}.{1}] {2}", contextOrdinal.ToString("D2", provider), stepOrdinal.ToString("D2", provider), stepName);

            string parameterSuffix = null;
            var argsArray = args.ToArray();
            var parameters = method.MethodInfo.GetParameters();
            if (parameters.Length > 0 || argsArray.Length > 0)
            {
                var tokens = new List<string>();
                for (var i = 0; i < Math.Max(parameters.Length, argsArray.Length); ++i)
                {
                    var parameter = parameters.ElementAtOrDefault(i);
                    var arg = argsArray.ElementAtOrDefault(i);
                    tokens.Add(string.Concat(parameter == null ? "???" : parameter.Name, ": ", (arg ?? "null").ToString()));
                }

                parameterSuffix = string.Concat("(", string.Join(", ", tokens.ToArray()), ")");
            }

            this.DisplayName = string.Format(CultureInfo.InvariantCulture, "{0}.{1}{2} {3}", method.TypeName, method.Name, parameterSuffix, this.name);
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