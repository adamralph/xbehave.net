// <copyright file="_.Expressions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using Xbehave.ExpressionNaming;
    using Xbehave.Fluent;
    using Xbehave.Infra;

    /// <summary>
    /// Provides a scenario step syntax an auto-generated step name.
    /// </summary>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "_", Justification = "By design.")]
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "By design.")]
    public static partial class _
    {
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="arrange">The action that will perform the arrangment.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        public static IStepDefinition Given(Expression<Action> arrange)
        {
            Require.NotNull(arrange, "arrange");
            return ("Given " + arrange.Body.ToStepName()).Given(arrange.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition Given(Expression<Func<IDisposable>> arrange)
        {
            Require.NotNull(arrange, "arrange");
            return ("Given " + arrange.Body.ToStepName()).Given(arrange.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition Given(Expression<Func<IEnumerable<IDisposable>>> arrange)
        {
            Require.NotNull(arrange, "arrange");
            return ("Given " + arrange.Body.ToStepName()).Given(arrange.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="arrange">The action that will perform the arrangement.</param>
        /// <param name="dispose">The action that will dispose the arrangement.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        public static IStepDefinition Given(Expression<Action> arrange, Action dispose)
        {
            Require.NotNull(arrange, "arrange");
            return ("Given " + arrange.Body.ToStepName()).Given(arrange.Compile(), dispose);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="act">The action that will perform the act.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        public static IStepDefinition When(Expression<Action> act)
        {
            Require.NotNull(act, "act");
            return ("When " + act.Body.ToStepName()).When(act.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        public static IStepDefinition Then(Expression<Action> assert)
        {
            Require.NotNull(assert, "assert");
            return ("Then " + assert.Body.ToStepName()).Then(assert.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        public static IStepDefinition ThenInIsolation(Expression<Action> assert)
        {
            Require.NotNull(assert, "assert");
            return ("Then " + assert.Body.ToStepName()).ThenInIsolation(assert.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="assert">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        public static IStepDefinition ThenSkip(Expression<Action> assert)
        {
            Require.NotNull(assert, "assert");
            return ("Then " + assert.Body.ToStepName()).ThenSkip(assert.Compile());
        }
    }
}
