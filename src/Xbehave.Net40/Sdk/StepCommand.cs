// <copyright file="StepCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Xunit.Sdk;

    internal class StepCommand : CommandBase
    {
        private readonly Step step;
        private readonly Action<IDisposable> handleResult;

        public StepCommand(MethodCall call, int? contextOrdinal, int ordinal, Step step, Action<IDisposable> handleResult)
            : base(call, contextOrdinal, ordinal, Format(call, contextOrdinal, ordinal, step))
        {
            this.step = step;
            this.handleResult = handleResult;
        }

        public override MethodResult Execute(object testClass)
        {
            if (this.step.SkipReason != null)
            {
                return new SkipResult(this.testMethod, this.DisplayName, this.step.SkipReason);
            }

            this.handleResult(this.step.Execute());
            return new PassedResult(this.testMethod, this.DisplayName);
        }

        private static string Format(MethodCall call, int? contextOrdinal, int ordinal, Step step)
        {
            var provider = CultureInfo.InvariantCulture;
            try
            {
                return string.Concat("\"", string.Format(provider, step.Name, call.Args), "\"");
            }
            catch (FormatException ex)
            {
                var message = string.Format(
                    provider,
                    "The name of step {0}{1}{2}, \"{3}\", could not be formatted using the scenario method argument{4} ({5}).",
                    ordinal,
                    contextOrdinal.HasValue ? " in context " : null,
                    contextOrdinal.HasValue ? contextOrdinal.Value.ToString(provider) : null,
                    step.Name,
                    call.Args.Count() == 1 ? null : "s",
                    call.Args.Count() == 0 ? "no arguments" : string.Join(", ", call.Args.Select(arg => arg.ToString()).ToArray()));

                throw new InvalidOperationException(message, ex);
            }
        }
    }
}