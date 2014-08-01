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
    public static class StepExtensions
    {
        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="step">The current step.</param>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "step", Justification = "Part of fluent API.")]
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.new Step(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep When(this IStep step, string text, Action body)
        {
            return new Step("When " + text, body);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="step">The current step.</param>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "step", Justification = "Part of fluent API.")]
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.new Step(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep Then(this IStep step, string text, Action body)
        {
            return new Step("Then " + text, body);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="step">The current step.</param>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "step", Justification = "Part of fluent API.")]
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.new Step(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep And(this IStep step, string text, Action body)
        {
            return new Step("And " + text, body);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="step">The current step.</param>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "step", Justification = "Part of fluent API.")]
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.new Step(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep But(this IStep step, string text, Action body)
        {
            return new Step("But " + text, body);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="step">The current step.</param>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "step", Justification = "Part of fluent API.")]
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.new Step(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep When(this IStep step, string text, Action<IStepContext> body)
        {
            return new Step("When " + text, body);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="step">The current step.</param>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "step", Justification = "Part of fluent API.")]
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.new Step(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep Then(this IStep step, string text, Action<IStepContext> body)
        {
            return new Step("Then " + text, body);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="step">The current step.</param>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "step", Justification = "Part of fluent API.")]
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.new Step(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep And(this IStep step, string text, Action<IStepContext> body)
        {
            return new Step("And " + text, body);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="step">The current step.</param>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "step", Justification = "Part of fluent API.")]
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "Xbehave.new Step(System.String,System.Action)",
            Justification = "Text must match method name.")]
        public static IStep But(this IStep step, string text, Action<IStepContext> body)
        {
            return new Step("But " + text, body);
        }
    }
}
