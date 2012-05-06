// <copyright file="_.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Linq.Expressions;
    using Xbehave.Fluent;
    using Xbehave.Infra;

    /// <summary>
    /// Extensions for declaring Given, When, Then scenario steps.
    /// </summary>
    public static partial class _
    {
        /// <summary>
        /// Deprecated in version 0.4.0.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use Given(Func<IDisposable>) instead.")]
        public static IStepDefinition GivenDisposable(this string message, ContextDelegate arrange)
        {
            Require.NotNull(arrange, "arrange");
            return message.Given(() => arrange());
        }

        /// <summary>
        /// Deprecated in version 0.10.0.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="assert">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use ThenSkip(reason, step) instead.")]
        public static IStepDefinition ThenSkip(this string message, Action assert)
        {
            return message.ThenSkip(assert, "Skipped for an unknown reason.");
        }

        /// <summary>
        /// Deprecated in version 0.10.0.
        /// </summary>
        /// <param name="assert">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use ThenSkip(reason, assert) instead.")]
        public static IStepDefinition ThenSkip(Expression<Action> assert)
        {
            return ThenSkip(assert, "Skipped for an unknown reason");
        }
    }
}
