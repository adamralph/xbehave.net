// <copyright file="IStepDefinition.Expressions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// The definition of a scenario step.
    /// </summary>
    public partial interface IStepDefinition
    {
        /// <summary>
        /// Deprecated in version 0.10.0.
        /// </summary>        
        /// <param name="assert">The action which would have performed the assertion.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use Then().Skip() instead.")]
        IStepDefinition ThenSkip(Expression<Action> assert);

        /// <summary>
        /// Deprecated in version 0.10.0.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use Then().InIsolation() instead.")]
        IStepDefinition ThenInIsolation(Expression<Action> body);
    }
}