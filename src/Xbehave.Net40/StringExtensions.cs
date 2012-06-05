// <copyright file="StringExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Fluent;

    /// <summary>
    /// Provides access to step definition methods.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "_", Justification = "By design.")]
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "By design.")]
    public static partial class StringExtensions
    {
        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <param name="teardown">An optional action which will perform teardown after execution of the scenario.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Given(this string text, Action body, Action teardown = null)
        {
            return Helper.AddStep("Given", text, body, teardown, false, null);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <param name="teardown">An optional action which will perform teardown after execution of the scenario.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition When(this string text, Action body, Action teardown = null)
        {
            return Helper.AddStep("When", text, body, teardown, false, null);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <param name="teardown">An optional action which will perform teardown after execution of the scenario.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Then(this string text, Action body, Action teardown = null)
        {
            return Helper.AddStep("Then", text, body, teardown, false, null);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <param name="teardown">An optional action which will perform teardown after execution of the scenario.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition And(this string text, Action body, Action teardown = null)
        {
            return Helper.AddStep("And", text, body, teardown, false, null);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <param name="teardown">An optional action which will perform teardown after execution of the scenario.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition But(this string text, Action body, Action teardown = null)
        {
            return Helper.AddStep("But", text, body, teardown, false, null);
        }
    }
}
