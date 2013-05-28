// <copyright file="StringExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>
namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Fluent;

    /// <summary>
    /// Provides access to step definition methods.
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        public static IStep Given(this string text, Action body)
        {
            return Helper.AddStep(text, body, StepType.Given);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        public static IStep When(this string text, Action body)
        {
            return Helper.AddStep(text, body, StepType.When);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        public static IStep Then(this string text, Action body)
        {
            return Helper.AddStep(text, body, StepType.Then);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        public static IStep And(this string text, Action body)
        {
            return Helper.AddStep(text, body, StepType.And);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        public static IStep But(this string text, Action body)
        {
            return Helper.AddStep(text, body, StepType.But);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        public static IStep f(this string text, Action body)
        {
            var stepType = GetStepType(text);
            return Helper.AddStep(text, body, stepType);
        }

        /// <summary>
        /// Get the appropriate step type based on the the text.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <returns>The appropriate step type based on the text.</returns>
        public static Xbehave.StepType GetStepType(string text)
        {
            var stepType = StepType.Any;
            if (text == null)
            {
                return stepType;
            }

            var upperText = text.ToUpper();

            if (upperText.StartsWith("GIVEN"))
            {
                stepType = StepType.Given;
            }
            else if (upperText.StartsWith("WHEN"))
            {
                stepType = StepType.When;
            }
            else if (upperText.StartsWith("THEN"))
            {
                stepType = StepType.Then;
            }
            else if (upperText.StartsWith("AND"))
            {
                stepType = StepType.And;
            }
            else if (upperText.StartsWith("BUT"))
            {
                stepType = StepType.But;
            }

            return stepType;
        }
    }
}
