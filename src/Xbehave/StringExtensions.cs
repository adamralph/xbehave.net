// <copyright file="StringExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Xbehave.Fluent;
    using Xbehave.Internal;

    /// <summary>
    /// Extensions for declaring Given, When, Then scenario steps.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Records the arrangement for this specification.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The action that will perform the arrangment.</param>
        /// <returns>An instance of <see cref="IGiven"/>.</returns>
        public static IGiven Given(this string message, Action arrange)
        {
            var step = ScenarioContext.Given(
                message,
                () =>
                {
                    arrange();
                    return Disposable.Empty;
                });

            return new Given(step);
        }

        /// <summary>
        /// Records the disposable arrangement for this specification which will be disposed after all associated assertions have been executed.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IGiven"/>.</returns>
        public static IGiven Given(this string message, Func<IDisposable> arrange)
        {
            var step = ScenarioContext.Given(
                message,
                () => arrange());

            return new Given(step);
        }

        /// <summary>
        /// Records the disposable arrangement for this specification which will be disposed after all associated assertions have been executed.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IGiven"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IGiven Given(this string message, Func<IEnumerable<IDisposable>> arrange)
        {
            var step = ScenarioContext.Given(
                message,
                () =>
                {
                    var disposables = arrange();
                    return new Disposable(disposables);
                });

            return new Given(step);
        }

        /// <summary>
        /// Records the disposable arrangement for this specification which will be disposed after all associated assertions have been executed.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The action that will perform the arrangement.</param>
        /// <param name="dispose">The action that will dispose the arrangement.</param>
        /// <returns>An instance of <see cref="IGiven"/>.</returns>
        public static IGiven Given(this string message, Action arrange, Action dispose)
        {
            var step = ScenarioContext.Given(
                message,
                () =>
                {
                    arrange();
                    return new Disposable(dispose);
                });

            return new Given(step);
        }

        /// <summary>
        /// Deprecated in version 0.4.0.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="ISpecificationPrimitive"/>.</returns>
        [Obsolete("Use Given(Func<IDisposable>) instead.")]
        public static IStep GivenDisposable(this string message, ContextDelegate arrange)
        {
            return message.ContextFixture(arrange);
        }

        /// <summary>
        /// Records the act to be performed on the arrangment for this specification.
        /// </summary>
        /// <param name="message">A message describing the act.</param>
        /// <param name="act">The action that will perform the act.</param>
        /// <returns>An instance of <see cref="IWhen"/>.</returns>
        public static IWhen When(this string message, Action act)
        {
            return new When(ScenarioContext.When(message, act));
        }

        /// <summary>
        /// Records an assertion of an expected outcome for this specification, to be executed on an isolated arrangement and action.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        public static IThen ThenInIsolation(this string message, Action assert)
        {
            return new Then(ScenarioContext.ThenInIsolation(message, assert));
        }

        /// <summary>
        /// Records an assertion of an expected outcome for this specification, to be executed on a shared arrangement and action.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        public static IThen Then(this string message, Action assert)
        {
            return new Then(ScenarioContext.Then(message, assert));
        }

        /// <summary>
        /// Records a skipped assertion of an expected outcome for this specification.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="assert">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        public static IThen ThenSkip(this string message, Action assert)
        {
            return new Then(ScenarioContext.ThenSkip(message, assert));
        }

        private class Disposable : IDisposable
        {
            private static readonly Disposable empty = new Disposable(() => { });

            private readonly Action dispose;
            private readonly IEnumerable<IDisposable> disposables;

            public Disposable(Action dispose)
            {
                this.dispose = dispose;
            }

            public Disposable(IEnumerable<IDisposable> disposables)
            {
                this.disposables = disposables;
            }

            public static Disposable Empty
            {
                get { return empty; }
            }

            public void Dispose()
            {
                if (this.dispose != null)
                {
                    this.dispose();
                }

                if (this.disposables != null)
                {
                    foreach (var disposable in this.disposables)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }
    }
}
