// <copyright file="_.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    using Xbehave.Fluent;
    using Xbehave.Internal;
    using Xbehave.Naming;

    /// <summary>
    /// Provides a scenario step syntax an auto-generated step name.
    /// </summary>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "_", Justification = "By design.")]
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "By design.")]
    public static class _
    {
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="arrange">The action that will perform the arrangment.</param>
        /// <returns>An instance of <see cref="IGiven"/>.</returns>
        public static IGiven Given(Expression<Action> arrange)
        {
            Require.NotNull(arrange, "arrange");
            return arrange.Body.ToGivenName().Given(arrange.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IGiven"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IGiven Given(Expression<Func<IDisposable>> arrange)
        {
            Require.NotNull(arrange, "arrange");
            return arrange.Body.ToGivenName().Given(arrange.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IGiven"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IGiven Given(Expression<Func<IEnumerable<IDisposable>>> arrange)
        {
            Require.NotNull(arrange, "arrange");
            return arrange.Body.ToGivenName().Given(arrange.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="arrange">The action that will perform the arrangement.</param>
        /// <param name="dispose">The action that will dispose the arrangement.</param>
        /// <returns>An instance of <see cref="IGiven"/>.</returns>
        public static IGiven Given(Expression<Action> arrange, Action dispose)
        {
            Require.NotNull(arrange, "arrange");
            return arrange.Body.ToGivenName().Given(arrange.Compile(), dispose);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="act">The action that will perform the act.</param>
        /// <returns>An instance of <see cref="IWhen"/>.</returns>
        public static IWhen When(Expression<Action> act)
        {
            Require.NotNull(act, "act");
            return act.Body.ToWhenName().When(act.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        public static IThen Then(Expression<Action> assert)
        {
            Require.NotNull(assert, "assert");
            return assert.Body.ToThenName().Then(assert.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        public static IThen ThenInIsolation(Expression<Action> assert)
        {
            Require.NotNull(assert, "assert");
            return assert.Body.ToThenName().ThenInIsolation(assert.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="assert">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        public static IThen ThenSkip(Expression<Action> assert)
        {
            Require.NotNull(assert, "assert");
            return assert.Body.ToThenName().ThenSkip(assert.Compile());
        }
    }
}
