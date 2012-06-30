// <copyright file="IStepDefinition.Expressions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    /// <summary>
    /// The definition of a scenario step.
    /// </summary>
    public partial interface IStepDefinition
    {
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "When", Justification = "By design.")]
        IStepDefinition When(Expression<Action> body);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Then", Justification = "By design.")]
        IStepDefinition Then(Expression<Action> body);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "By design.")]
        IStepDefinition And(Expression<Action> body);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        IStepDefinition But(Expression<Action> body);
    }
}