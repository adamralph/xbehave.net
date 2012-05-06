// <copyright file="DisposalCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Xbehave.Infra;
    using Xunit.Sdk;

    internal class DisposalCommand : TestCommand
    {
        private readonly IDisposer disposer = new Disposer();
        private readonly IEnumerable<IDisposable> disposables;

        public DisposalCommand(MethodCall call, int ordinal, string context, IEnumerable<IDisposable> disposables)
            : base(call.Method, CreateDisposalCommandName(call, ordinal, context), MethodUtility.GetTimeoutParameter(call.Method))
        {
            this.disposables = disposables;
        }

        public override MethodResult Execute(object testClass)
        {
            this.disposer.Dispose(this.disposables);
            return new PassedResult(this.testMethod, this.DisplayName);
        }

        private static string CreateDisposalCommandName(MethodCall call, int stepOrdinal, string context)
        {
            return string.Concat(
                call.ToString(),
                context == null ? null : "(" + context + ")",
                ".",
                stepOrdinal.ToString("D2", CultureInfo.InvariantCulture),
                ".",
                "Disposal");
        }
    }
}