// <copyright file="IStepDefinition.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// The definition of a scenario step.
    /// </summary>
    public interface IStepDefinition
    {
        /// <summary>
        /// Indicate that execution of the defined step should be cancelled after a specified timeout.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="System.Threading.Timeout.Infinite"/> (-1) to wait indefinitely.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        IStepDefinition WithTimeout(int millisecondsTimeout);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="act">The action that will perform the act.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        IStepDefinition When(Expression<Action> act);
        
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        IStepDefinition Then(Expression<Action> assert);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        IStepDefinition ThenInIsolation(Expression<Action> assert);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="assert">The action which would have performed the assertion.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        IStepDefinition ThenSkip(Expression<Action> assert);
    }
}