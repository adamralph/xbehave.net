// <copyright file="StringExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using Xbehave.Fluent;
    using Xbehave.Sdk.Infra;

    /// <summary>
    /// Extensions for declaring Given, When, Then scenario steps.
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// Deprecated in version 0.4.0.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="body">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use Given(Func<IDisposable>) instead.")]
        public static IStepDefinition GivenDisposable(this string message, ContextDelegate body)
        {
            Require.NotNull(body, "arrange");
            return message.Given(() => body());
        }

        /// <summary>
        /// Deprecated in version 0.10.0.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="body">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use ThenSkip(reason, step) instead.")]
        public static IStepDefinition ThenSkip(this string message, Action body)
        {
            return message.ThenSkip(body, "Skipped for an unknown reason.");
        }
    }
}
