// <copyright file="StepExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Fluent;

    /// <summary>
    /// <see cref="IStep"/> extensions.
    /// </summary>
    public static partial class StepExtensions
    {
        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "stepDefinition", Justification = "Part of fluent API.")]
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.Helper.AddStep(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep When(this IStep stepDefinition, string text, Action body)
        {
            return Helper.AddStep("When " + text, body);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "stepDefinition", Justification = "Part of fluent API.")]
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.Helper.AddStep(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep Then(this IStep stepDefinition, string text, Action body)
        {
            return Helper.AddStep("Then " + text, body);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "stepDefinition", Justification = "Part of fluent API.")]
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.Helper.AddStep(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep And(this IStep stepDefinition, string text, Action body)
        {
            return Helper.AddStep("And " + text, body);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "stepDefinition", Justification = "Part of fluent API.")]
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.Helper.AddStep(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep But(this IStep stepDefinition, string text, Action body)
        {
            return Helper.AddStep("But " + text, body);
        }
    }
}
