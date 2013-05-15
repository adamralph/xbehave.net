// <copyright file="_.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Sdk;

    using Xbehave.Fluent;

    /// <summary>
    /// Provides access to step definition methods.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "_", Justification = "By design.")]
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "By design.")]
    [CLSCompliant(false)]
    public static partial class _
    {
        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.Helper.AddStep(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep Given(string text, Action body)
        {
            return Helper.AddStep("Given " + text, body, StepType.Given);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.Helper.AddStep(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep When(string text, Action body)
        {
            return Helper.AddStep("When " + text, body, StepType.When);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.Helper.AddStep(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep Then(string text, Action body)
        {
            return Helper.AddStep("Then " + text, body, StepType.Then);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.Helper.AddStep(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep And(string text, Action body)
        {
            return Helper.AddStep("And " + text, body, StepType.And);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.Helper.AddStep(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep But(string text, Action body)
        {
            return Helper.AddStep("But " + text, body, StepType.But);
        }
    }
}
