// <copyright file="_.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Xbehave.Fluent;
    using Xbehave.Naming;

    /// <summary>
    /// Provides fluent specification syntax with auto-generated specification names.
    /// </summary>
    public static class _
    {
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="arrange">The action that will perform the arrangment.</param>
        /// <returns>An instance of <see cref="IGiven"/>.</returns>
        public static IGiven Given(Expression<Action> arrange)
        {
            ValidateArrange(arrange);
            return arrange.Body.ToGivenName().Given(arrange.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IGiven"/>.</returns>
        public static IGiven Given(Expression<Func<IDisposable>> arrange)
        {
            ValidateArrange(arrange);
            return arrange.Body.ToGivenName().Given(arrange.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IGiven"/>.</returns>
        public static IGiven Given(Expression<Func<IEnumerable<IDisposable>>> arrange)
        {
            ValidateArrange(arrange);
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
            ValidateArrange(arrange);
            return arrange.Body.ToGivenName().Given(arrange.Compile(), dispose);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="act">The action that will perform the act.</param>
        /// <returns>An instance of <see cref="IWhen"/>.</returns>
        public static IWhen When(Expression<Action> act)
        {
            ValidateAct(act);
            return act.Body.ToWhenName().When(act.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        public static IThen Then(Expression<Action> assert)
        {
            ValidateAssert(assert);
            return assert.Body.ToThenName().Then(assert.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        public static IThen ThenInIsolation(Expression<Action> assert)
        {
            ValidateAssert(assert);
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
            ValidateAssert(assert);
            return assert.Body.ToThenName().ThenSkip(assert.Compile());
        }

        private static void ValidateArrange(object arrange)
        {
            if (arrange == null)
            {
                throw new ArgumentNullException("arrange");
            }
        }

        private static void ValidateAct(object act)
        {
            if (act == null)
            {
                throw new ArgumentNullException("act");
            }
        }

        private static void ValidateAssert(object assert)
        {
            if (assert == null)
            {
                throw new ArgumentNullException("assert");
            }
        }
    }
}
