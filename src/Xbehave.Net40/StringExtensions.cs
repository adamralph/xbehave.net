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
        /// Defines an arrangement for the scenario.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The action that will perform the arrangment.</param>
        /// <returns>An instance of <see cref="IGivenDefinition"/>.</returns>
        public static IGivenDefinition Given(this string message, Action arrange)
        {
            return new GivenDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(arrange))));
        }

        /// <summary>
        /// Defines a disposable arrangement for the scenario which will be disposed after all steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IGivenDefinition"/>.</returns>
        public static IGivenDefinition Given(this string message, Func<IDisposable> arrange)
        {
            return new GivenDefinition(CurrentThread.Scenario.Enqueue(new Step(message, arrange)));
        }

        /// <summary>
        /// Defines a disposable arrangement for the scenario which will be disposed after all steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IGivenDefinition"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IGivenDefinition Given(this string message, Func<IEnumerable<IDisposable>> arrange)
        {
            return new GivenDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(arrange))));
        }

        /// <summary>
        /// Defines a disposable arrangement for the scenario which will be disposed after all steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The action that will perform the arrangement.</param>
        /// <param name="dispose">The action that will dispose the arrangement.</param>
        /// <returns>An instance of <see cref="IGivenDefinition"/>.</returns>
        public static IGivenDefinition Given(this string message, Action arrange, Action dispose)
        {
            return new GivenDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(arrange, dispose))));
        }

        /// <summary>
        /// Defines an act to be performed in the scenario.
        /// </summary>
        /// <param name="message">A message describing the act.</param>
        /// <param name="act">The action that will perform the act.</param>
        /// <returns>An instance of <see cref="IWhenDefinition"/>.</returns>
        public static IWhenDefinition When(this string message, Action act)
        {
            return new WhenDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(act))));
        }

        /// <summary>
        /// Defines an assertion of an expected outcome of the scenario with all arrangements and actions isolated for this assertion.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThenDefinition"/>.</returns>
        public static IThenDefinition ThenInIsolation(this string message, Action assert)
        {
            return new ThenDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(assert), true)));
        }

        /// <summary>
        /// Defines an assertion of an expected outcome of the scenario with all arrangements and actions shared with other assertions.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IThenDefinition"/>.</returns>
        public static IThenDefinition Then(this string message, Action assert)
        {
            return new ThenDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(assert))));
        }

        /// <summary>
        /// Defines a skipped assertion of an expected outcome for the scenario.
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
            return new ThenDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(assert), "Skipped for an unknown reason.")));
        }
    }
}
