// <copyright file="DisposalStep.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Sdk.Infrastructure;

    public class DisposalStep : Step
    {
        [SuppressMessage(
            "Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.Sdk.Step.#ctor(System.String,System.String,System.Action)",
            Justification = "Localization not required.")]
        public DisposalStep(IEnumerable<IDisposable> disposables)
            : base("Disposal and/or teardown", disposables.DisposeAll)
        {
        }
    }
}