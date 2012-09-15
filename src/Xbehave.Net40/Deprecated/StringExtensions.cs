// <copyright file="StringExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using Xbehave.Fluent;

    /// <summary>
    /// Extensions for declaring Given, When, Then scenario steps.
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// Deprecated in version 0.4.0.
        /// </summary>
        /// <param name="text">A text describing the arrangement.</param>
        /// <param name="body">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [Obsolete("Use Given().Teardown() instead.")]
        public static IStep GivenDisposable(this string text, ContextDelegate body)
        {
            return text.Given(() => body().Using());
        }

        /// <summary>
        /// Deprecated in version 0.10.0.
        /// </summary>
        /// <param name="text">A text describing the assertion.</param>
        /// <param name="body">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [Obsolete("Use Then().Skip() instead.")]
        public static IStep ThenSkip(this string text, Action body)
        {
            return text.Then(body).Skip("Skipped for an unknown reason.");
        }

        /// <summary>
        /// Deprecated in version 0.10.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [Obsolete("Use Then().InIsolation() instead.")]
        public static IStep ThenInIsolation(this string text, Action body)
        {
            return text.Then(body).InIsolation();
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <param name="dispose">An optional action which will perform teardown after execution of the scenario.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [Obsolete("Use Given().Teardown() instead.")]
        public static IStep Given(this string text, Action body, Action dispose)
        {
            return Helper.AddStep(text, body).Teardown(dispose);
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <param name="dispose">An optional action which will perform teardown after execution of the scenario.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [Obsolete("Use When().Teardown() instead.")]
        public static IStep When(this string text, Action body, Action dispose)
        {
            return Helper.AddStep(text, body).Teardown(dispose);
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <param name="dispose">An optional action which will perform teardown after execution of the scenario.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [Obsolete("Use Then().Teardown() instead.")]
        public static IStep Then(this string text, Action body, Action dispose)
        {
            return Helper.AddStep(text, body).Teardown(dispose);
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <param name="dispose">An optional action which will perform teardown after execution of the scenario.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [Obsolete("Use And().Teardown() instead.")]
        public static IStep And(this string text, Action body, Action dispose)
        {
            return Helper.AddStep(text, body).Teardown(dispose);
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <param name="dispose">An optional action which will perform teardown after execution of the scenario.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [Obsolete("Use But().Teardown() instead.")]
        public static IStep But(this string text, Action body, Action dispose)
        {
            return Helper.AddStep(text, body).Teardown(dispose);
        }
    }
}
