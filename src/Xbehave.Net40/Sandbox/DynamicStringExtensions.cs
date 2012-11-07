// <copyright file="DynamicStringExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>
#if NET40

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Xbehave.Fluent;
    using Xbehave.Sdk;

    /// <summary>
    /// Extensions for declaring Given, When, Then scenario steps.
    /// </summary>
    public static partial class DynamicStringExtensions
    {
        /// <summary>
        /// Records the arrangement for this specification.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The action that will perform the arrangment.</param>
        /// <returns>An instance of <see cref="IGiven"/>.</returns>
        public static IGiven Given(this string message, Action<dynamic> arrange)
        {
            Require.NotNull(arrange, "arrange");

            var step = ThreadContext.Scenario.Given(
                message,
                () =>
                {
                    arrange(ThreadContext.Scenario.Context);
                    return null;
                });

            return new Given(step);
        }

        /// <summary>
        /// Records the disposable arrangement for this specification which will be disposed after all associated assertions have been executed.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IGiven"/>.</returns>
        public static IGiven Given(this string message, Func<dynamic, IDisposable> arrange)
        {
            Require.NotNull(arrange, "arrange");

            var step = ThreadContext.Scenario.Given(message, () => arrange(ThreadContext.Scenario.Context));
            return new Given(step);
        }

        /// <summary>
        /// Records the disposable arrangement for this specification which will be disposed after all associated assertions have been executed.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IGiven"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IGiven Given(this string message, Func<dynamic, IEnumerable<IDisposable>> arrange)
        {
            Require.NotNull(arrange, "arrange");

            var step = ThreadContext.Scenario.Given(
                message,
                () =>
                {
                    var disposables = arrange(ThreadContext.Scenario.Context);
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
        public static IGiven Given(this string message, Action<dynamic> arrange, Action<dynamic> dispose)
        {
            Require.NotNull(arrange, "arrange");

            var step = ThreadContext.Scenario.Given(
                message,
                () =>
                {
                    arrange(ThreadContext.Scenario.Context);
                    return new Disposable(() => dispose(ThreadContext.Scenario.Context));
                });

            return new Given(step);
        }

        /// <summary>
        /// Records the act to be performed on the arrangment for this specification.
        /// </summary>
        /// <param name="message">A message describing the act.</param>
        /// <param name="act">The action that will perform the act.</param>
        /// <returns>An instance of <see cref="IWhen"/>.</returns>
        public static IWhen When(this string message, Action<dynamic> act)
        {
            return new When(ThreadContext.Scenario.When(message, ActionExtensions.ToDefaultFunc<IDisposable>(() => act(ThreadContext.Scenario.Context))));
        }

        /// <summary>
        /// Records an assertion of an expected outcome for this specification, to be executed on an isolated arrangement and action.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        public static IThen ThenInIsolation(this string message, Action<dynamic> assert)
        {
            return new Then(ThreadContext.Scenario.ThenInIsolation(message, ActionExtensions.ToDefaultFunc<IDisposable>(() => assert(ThreadContext.Scenario.Context))));
        }

        /// <summary>
        /// Records an assertion of an expected outcome for this specification, to be executed on a shared arrangement and action.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        public static IThen Then(this string message, Action<dynamic> assert)
        {
            return new Then(ThreadContext.Scenario.Then(message, ActionExtensions.ToDefaultFunc<IDisposable>(() => assert(ThreadContext.Scenario.Context))));
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
        public static IThen ThenSkip(this string message, Action<dynamic> assert)
        {
            return new Then(ThreadContext.Scenario.ThenSkip(message, ActionExtensions.ToDefaultFunc<IDisposable>(() => assert(ThreadContext.Scenario.Context))));
        }
    }
}
#endif
