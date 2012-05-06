// <copyright file="IStepDefinition.Expressions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Collections.Generic;
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
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "When", Justification = "By design.")]
        IStepDefinition When(Expression<Action> step);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "When", Justification = "By design.")]
        IStepDefinition When(Expression<Func<IDisposable>> step);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "When", Justification = "By design.")]
        IStepDefinition When(Expression<Func<IEnumerable<IDisposable>>> step);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="dispose">The dispose.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "When", Justification = "By design.")]
        IStepDefinition When(Expression<Action> step, Action dispose);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The action which will perform the assertion.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Then", Justification = "By design.")]
        IStepDefinition Then(Expression<Action> step);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        IStepDefinition ThenInIsolation(Expression<Action> step);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="reason">The reason.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        IStepDefinition ThenSkip(Expression<Action> step, string reason);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "By design.")]
        IStepDefinition And(Expression<Action> step);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "By design.")]
        IStepDefinition And(Expression<Func<IDisposable>> step);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "By design.")]
        IStepDefinition And(Expression<Func<IEnumerable<IDisposable>>> step);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="dispose">The dispose.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "By design.")]
        IStepDefinition And(Expression<Action> step, Action dispose);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        IStepDefinition AndInIsolation(Expression<Action> step);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="reason">The reason.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        IStepDefinition AndSkip(Expression<Action> step, string reason);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The action which will perform the assertion.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        IStepDefinition But(Expression<Action> step);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        IStepDefinition ButInIsolation(Expression<Action> step);

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="reason">The reason.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        IStepDefinition ButSkip(Expression<Action> step, string reason);
    }
}