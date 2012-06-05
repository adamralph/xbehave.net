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
    using Xbehave.Infra;

    /// <summary>
    /// Extensions for declaring Given, When, Then scenario steps.
    /// </summary>
    public static partial class _
    {
        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Given(string text, Func<IDisposable> body)
        {
            IDisposable disposable = null;
            return text.Given(() => disposable = body(), () => new Disposable(new[] { disposable }).Dispose());
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The function that will perform the step and return the disposable resources.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use IDisposable.Using() instead.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition Given(string text, Func<IEnumerable<IDisposable>> body)
        {
            IEnumerable<IDisposable> disposables = null;
            return text.Given(() => disposables = body(), () => new Disposable(disposables).Dispose());
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use IDisposable.Using() instead.")]
        public static IStepDefinition When(string text, Func<IDisposable> body)
        {
            IDisposable disposable = null;
            return text.When(() => disposable = body(), () => new Disposable(new[] { disposable }).Dispose());
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The function that will perform the step and return the disposable resources.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use IDisposable.Using() instead.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition When(string text, Func<IEnumerable<IDisposable>> body)
        {
            IEnumerable<IDisposable> disposables = null;
            return text.When(() => disposables = body(), () => new Disposable(disposables).Dispose());
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use IDisposable.Using() instead.")]
        public static IStepDefinition And(string text, Func<IDisposable> body)
        {
            IDisposable disposable = null;
            return text.And(() => disposable = body(), () => new Disposable(new[] { disposable }).Dispose());
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The function that will perform the step and return the disposable resources.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use IDisposable.Using() instead.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition And(string text, Func<IEnumerable<IDisposable>> body)
        {
            IEnumerable<IDisposable> disposables = null;
            return text.And(() => disposables = body(), () => new Disposable(disposables).Dispose());
        }
    }
}
