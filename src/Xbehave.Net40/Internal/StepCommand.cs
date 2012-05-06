// <copyright file="StepCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Globalization;
    using Xunit.Sdk;

    internal class StepCommand : TestCommand
    {
        private readonly Action<IDisposable> handleResult;
        private readonly Step step;

        public StepCommand(MethodCall call, int ordinal, Step step, string context, Action<IDisposable> handleResult)
            : base(call.Method, CreateCommandName(call, ordinal, step.Message, context), MethodUtility.GetTimeoutParameter(call.Method))
        {
            this.handleResult = handleResult;
            this.step = step;
        }

        public override MethodResult Execute(object testClass)
        {
            if (this.step.Skip)
            {
                return new SkipResult(this.testMethod, this.DisplayName, this.step.SkipReason);
            }

            this.handleResult(this.step.Execute());
            return new PassedResult(this.testMethod, this.DisplayName);
        }

        private static string CreateCommandName(MethodCall call, int stepOrdinal, string stepName, string context)
        {
            return string.Concat(
                call.ToString(),
                context == null ? null : "(" + context + ")",
                ".",
                stepOrdinal.ToString("D2", CultureInfo.InvariantCulture),
                ".",
                "\"",
                stepName,
                "\"");
        }
    }
}