// <copyright file="StepCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Globalization;
    using Xunit.Sdk;

    internal class StepCommand : CommandBase
    {
        private readonly Step step;
        private readonly Action<IDisposable> handleResult;

        public StepCommand(MethodCall call, int? contextOrdinal, int ordinal, Step step, Action<IDisposable> handleResult)
            : base(call, contextOrdinal, ordinal, Format(call, step))
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

        private static string Format(MethodCall call, Step step)
        {
            string stepName;
            try
            {
                stepName = string.Format(CultureInfo.InvariantCulture, step.Name, call.Args);
            }
            catch (FormatException)
            {
                stepName = step.Name;
            }

            return string.Concat("\"", stepName.Replace("\"", "\\\""), "\"");
        }
    }
}