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
        /// Declares an arrangement for the scenario.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The action that will perform the arrangment.</param>
        /// <returns>An instance of <see cref="IGivenDefinition"/>.</returns>
        public static IGivenDefinition Given(this string message, Action arrange)
        {
            return new GivenDefinition(ThreadContext.Scenario.Given(new Step(message, DisposableFunctionFactory.Create(arrange))));
        }

        /// <summary>
        /// Declares a disposable arrangement for the scenario which will be disposed after all steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IGivenDefinition"/>.</returns>
        public static IGivenDefinition Given(this string message, Func<IDisposable> arrange)
        {
            return new GivenDefinition(ThreadContext.Scenario.Given(new Step(message, arrange)));
        }

        /// <summary>
        /// Declares a disposable arrangement for the scenario which will be disposed after all steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IGivenDefinition"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IGivenDefinition Given(this string message, Func<IEnumerable<IDisposable>> arrange)
        {
            return new GivenDefinition(ThreadContext.Scenario.Given(new Step(message, DisposableFunctionFactory.Create(arrange))));
        }

        /// <summary>
        /// Declares a disposable arrangement for the scenario which will be disposed after all steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The action that will perform the arrangement.</param>
        /// <param name="dispose">The action that will dispose the arrangement.</param>
        /// <returns>An instance of <see cref="IGivenDefinition"/>.</returns>
        public static IGivenDefinition Given(this string message, Action arrange, Action dispose)
        {
            return new GivenDefinition(ThreadContext.Scenario.Given(new Step(message, DisposableFunctionFactory.Create(arrange, dispose))));
        }

        /// <summary>
        /// Declares an act to be performed in the scenario.
        /// </summary>
        /// <param name="message">A message describing the act.</param>
        /// <param name="act">The action that will perform the act.</param>
        /// <returns>An instance of <see cref="IWhenDefinition"/>.</returns>
        public static IWhenDefinition When(this string message, Action act)
        {
            return new WhenDefinition(ThreadContext.Scenario.When(new Step(message, DisposableFunctionFactory.Create(act))));
        }

        /// <summary>
        /// Records an assertion of an expected outcome of the scenario with all arrangements and actions isolated for this assertion.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThenDefinition"/>.</returns>
        public static IThenDefinition ThenInIsolation(this string message, Action assert)
        {
            return new ThenDefinition(ThreadContext.Scenario.ThenInIsolation(new Step(message, DisposableFunctionFactory.Create(assert))));
        }

        /// <summary>
        /// Records an assertion of an expected outcome of the scenario with all arrangements and actions shared with other assertions.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThenDefinition"/>.</returns>
        public static IThenDefinition Then(this string message, Action assert)
        {
            return new ThenDefinition(ThreadContext.Scenario.Then(new Step(message, DisposableFunctionFactory.Create(assert))));
        }

        /// <summary>
        /// Records a skipped assertion of an expected outcome for the scenario.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="assert">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IThenDefinition"/>.</returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        public static IThenDefinition ThenSkip(this string message, Action assert)
        {
            return new ThenDefinition(ThreadContext.Scenario.ThenSkip(new Step(message, DisposableFunctionFactory.Create(assert))));
        }
    }
}
