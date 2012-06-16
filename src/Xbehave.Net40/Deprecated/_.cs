// <copyright file="_.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
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
            return text.Given(() => disposable = body(), () => new[] { disposable }.DisposeAll());
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The function that will perform the step and return the disposable resources.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use Given(Action body, Action teardown) instead.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition Given(string text, Func<IEnumerable<IDisposable>> body)
        {
            IEnumerable<IDisposable> disposables = null;
            return text.Given(() => disposables = body(), () => (disposables ?? new IDisposable[0]).Reverse().DisposeAll());
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use When(Action body, Action teardown) instead.")]
        public static IStepDefinition When(string text, Func<IDisposable> body)
        {
            IDisposable disposable = null;
            return text.When(() => disposable = body(), () => new[] { disposable }.DisposeAll());
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The function that will perform the step and return the disposable resources.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use When(Action body, Action teardown) instead.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition When(string text, Func<IEnumerable<IDisposable>> body)
        {
            IEnumerable<IDisposable> disposables = null;
            return text.When(() => disposables = body(), () => (disposables ?? new IDisposable[0]).Reverse().DisposeAll());
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use And(Action body, Action teardown) instead.")]
        public static IStepDefinition And(string text, Func<IDisposable> body)
        {
            IDisposable disposable = null;
            return text.And(() => disposable = body(), () => new[] { disposable }.DisposeAll());
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The function that will perform the step and return the disposable resources.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use And(Action body, Action teardown) instead.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition And(string text, Func<IEnumerable<IDisposable>> body)
        {
            IEnumerable<IDisposable> disposables = null;
            return text.And(() => disposables = body(), () => (disposables ?? new IDisposable[0]).Reverse().DisposeAll());
        }
    }
}
