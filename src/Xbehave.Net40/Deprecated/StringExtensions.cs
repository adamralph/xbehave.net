// <copyright file="StringExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using Xbehave.Fluent;
    using Xbehave.Infra;

    /// <summary>
    /// Extensions for declaring Given, When, Then scenario steps.
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// Deprecated in version 0.4.0.
        /// </summary>
        /// <param name="text">A text describing the arrangment.</param>
        /// <param name="body">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use Given(Func<IDisposable>) instead.")]
        public static IStepDefinition GivenDisposable(this string text, ContextDelegate body)
        {
            Require.NotNull(body, "arrange");
            return text.Given(() => body());
        }

        /// <summary>
        /// Deprecated in version 0.10.0.
        /// </summary>
        /// <param name="text">A text describing the assertion.</param>
        /// <param name="body">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use Then().Skip() instead.")]
        public static IStepDefinition ThenSkip(this string text, Action body)
        {
            return text.Then(body).Skip("Skipped for an unknown reason.");
        }

        /// <summary>
        /// Deprecated in version 0.10.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use Then().InIsolation() instead.")]
        public static IStepDefinition ThenInIsolation(this string text, Action body)
        {
            return text.Then(body).InIsolation();
        }
    }
}
