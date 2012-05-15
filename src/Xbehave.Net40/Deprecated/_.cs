// <copyright file="_.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Linq.Expressions;
    using Xbehave.Fluent;

    /// <summary>
    /// Extensions for declaring Given, When, Then scenario steps.
    /// </summary>
    public static partial class _
    {
        /// <summary>
        /// Deprecated in version 0.10.0.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use Then().InIsolation() instead.")]
        public static IStepDefinition ThenInIsolation(Expression<Action> body)
        {
            return Then(body).InIsolation();
        }

        /// <summary>
        /// Deprecated in version 0.10.0.
        /// </summary>
        /// <param name="body">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use Then().Skip() instead.")]
        public static IStepDefinition ThenSkip(Expression<Action> body)
        {
            return Then(body).Skip("Skipped for an unknown reason");
        }
    }
}
