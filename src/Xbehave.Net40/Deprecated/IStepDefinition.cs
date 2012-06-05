// <copyright file="IStepDefinition.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The definition of a scenario step.
    /// </summary>
    public partial interface IStepDefinition
    {
        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use IDisposable.Using() instead.")]
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "When", Justification = "By design.")]
        IStepDefinition When(string text, Func<IDisposable> body);

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The function that will perform the step and return the disposable resources.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use IDisposable.Using() instead.")]
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "When", Justification = "By design.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        IStepDefinition When(string text, Func<IEnumerable<IDisposable>> body);

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use IDisposable.Using() instead.")]
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "By design.")]
        IStepDefinition And(string text, Func<IDisposable> body);

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The function that will perform the step and return the disposable resources.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use IDisposable.Using() instead.")]
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "By design.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        IStepDefinition And(string text, Func<IEnumerable<IDisposable>> body);
    }
}