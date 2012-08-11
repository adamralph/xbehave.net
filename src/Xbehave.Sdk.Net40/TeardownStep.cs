// <copyright file="TeardownStep.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Sdk.Infrastructure;

    public class TeardownStep : Step
    {
        [SuppressMessage(
            "Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.Sdk.Step.#ctor(System.String,System.String,System.Action)",
            Justification = "Localization not required.")]
        public TeardownStep(IEnumerable<Action> teardowns)
            : base("(Teardown)", teardowns.InvokeAll)
        {
        }
    }
}