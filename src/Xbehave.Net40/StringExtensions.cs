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
    public static partial class StringExtensions
    {
        /// <summary>
        /// Records the arrangement for this specification.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The action that will perform the arrangment.</param>
        /// <returns>An instance of <see cref="IGiven"/>.</returns>
        public static IGiven Given(this string message, Action arrange)
        {
            return new Given(ThreadContext.Scenario.Given(StepFactory.Create(message, arrange)));
        }

        /// <summary>
        /// Records the disposable arrangement for this specification which will be disposed after all associated assertions have been executed.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IGiven"/>.</returns>
        public static IGiven Given(this string message, Func<IDisposable> arrange)
        {
            return new Given(ThreadContext.Scenario.Given(StepFactory.Create(message, arrange)));
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
            return new Given(ThreadContext.Scenario.Given(StepFactory.Create(message, arrange)));
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
            return new Given(ThreadContext.Scenario.Given(StepFactory.Create(message, arrange, dispose)));
        }

        /// <summary>
        /// Records the act to be performed on the arrangment for this specification.
        /// </summary>
        /// <param name="message">A message describing the act.</param>
        /// <param name="act">The action that will perform the act.</param>
        /// <returns>An instance of <see cref="IWhen"/>.</returns>
        public static IWhen When(this string message, Action act)
        {
            return new When(ThreadContext.Scenario.When(StepFactory.Create(message, act)));
        }

        /// <summary>
        /// Records an assertion of an expected outcome for this specification, to be executed on an isolated arrangement and action.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        public static IThen ThenInIsolation(this string message, Action assert)
        {
            return new Then(ThreadContext.Scenario.ThenInIsolation(StepFactory.Create(message, assert)));
        }

        /// <summary>
        /// Records an assertion of an expected outcome for this specification, to be executed on a shared arrangement and action.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThen"/>.</returns>
        public static IThen Then(this string message, Action assert)
        {
            return new Then(ThreadContext.Scenario.Then(StepFactory.Create(message, assert)));
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
            return new Then(ThreadContext.Scenario.ThenSkip(StepFactory.Create(message, assert)));
        }
    }
}
