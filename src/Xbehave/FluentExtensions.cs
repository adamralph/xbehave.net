// <copyright file="FluentExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Linq.Expressions;

    using Xbehave.Fluent;

    /// <summary>
    /// Provides extensions for a fluent specification syntax with auto-generated specification names.
    /// </summary>
    public static class FluentExtensions
    {
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="act">The action that will perform the act.</param>
        /// <returns>An instance of <see cref="IWhen"/>.</returns>
        public static IWhen When(this IGiven spec, Expression<Action> act)
        {
            return _.When(act);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        public static IThen Then(this IWhen spec, Expression<Action> assert)
        {
            return _.Then(assert);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        public static IThen ThenInIsolation(this IWhen spec, Expression<Action> assert)
        {
            return _.ThenInIsolation(assert);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="assert">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        public static IThen ThenSkip(this IWhen spec, Expression<Action> assert)
        {
            return _.ThenSkip(assert);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        public static IThen Then(this IThen spec, Expression<Action> assert)
        {
            return _.Then(assert);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        public static IThen ThenInIsolation(this IThen spec, Expression<Action> assert)
        {
            return _.ThenInIsolation(assert);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="assert">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        public static IThen ThenSkip(this IThen spec, Expression<Action> assert)
        {
            return _.ThenSkip(assert);
        }
    }
}
