// <copyright file="_.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Linq.Expressions;
    using Xbehave.Sdk.Fluent;

    /// <summary>
    /// Extensions for declaring Given, When, Then scenario steps.
    /// </summary>
    public static partial class _
    {
        /// <summary>
        /// Deprecated in version 0.10.0.
        /// </summary>
        /// <param name="body">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use ThenSkip(reason, assert) instead.")]
        public static IStepDefinition ThenSkip(Expression<Action> body)
        {
            return ThenSkip(body, "Skipped for an unknown reason");
        }
    }
}
