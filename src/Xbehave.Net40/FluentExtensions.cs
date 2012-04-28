// <copyright file="FluentExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    using Xbehave.Fluent;

    /// <summary>
    /// Extensions for a fluent scenario step syntax with auto-generated step names.
    /// </summary>
    public static class FluentExtensions
    {
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="act">The action that will perform the act.</param>
        /// <returns>
        /// An instance of <see cref="IWhenDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "step", Justification = "Required for fluent syntax.")]
        public static IWhenDefinition When(this IGivenDefinition step, Expression<Action> act)
        {
            return _.When(act);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>
        /// An instance of <see cref="IThenDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "step", Justification = "Required for fluent syntax.")]
        public static IThenDefinition Then(this IWhenDefinition step, Expression<Action> assert)
        {
            return _.Then(assert);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThenDefinition"/>.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "step", Justification = "Required for fluent syntax.")]
        public static IThenDefinition ThenInIsolation(this IWhenDefinition step, Expression<Action> assert)
        {
            return _.ThenInIsolation(assert);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="assert">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IThenDefinition"/>.</returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "step", Justification = "Required for fluent syntax.")]
        public static IThenDefinition ThenSkip(this IWhenDefinition step, Expression<Action> assert)
        {
            return _.ThenSkip(assert);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThenDefinition"/>.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "step", Justification = "Required for fluent syntax.")]
        public static IThenDefinition Then(this IThenDefinition step, Expression<Action> assert)
        {
            return _.Then(assert);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThenDefinition"/>.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "step", Justification = "Required for fluent syntax.")]
        public static IThenDefinition ThenInIsolation(this IThenDefinition step, Expression<Action> assert)
        {
            return _.ThenInIsolation(assert);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="assert">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IThenDefinition"/>.</returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "step", Justification = "Required for fluent syntax.")]
        public static IThenDefinition ThenSkip(this IThenDefinition step, Expression<Action> assert)
        {
            return _.ThenSkip(assert);
        }
    }
}
