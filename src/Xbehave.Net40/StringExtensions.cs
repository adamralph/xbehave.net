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
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        public static IStepDefinition Given(this string message, Action arrange)
        {
            return new StepDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(arrange))));
        }

        /// <summary>
        /// Defines a disposable arrangement for the scenario which will be disposed after all steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        public static IStepDefinition Given(this string message, Func<IDisposable> arrange)
        {
            return new StepDefinition(CurrentThread.Scenario.Enqueue(new Step(message, arrange)));
        }

        /// <summary>
        /// Defines a disposable arrangement for the scenario which will be disposed after all steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition Given(this string message, Func<IEnumerable<IDisposable>> arrange)
        {
            return new StepDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(arrange))));
        }

        /// <summary>
        /// Defines a disposable arrangement for the scenario which will be disposed after all steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The action that will perform the arrangement.</param>
        /// <param name="dispose">The action that will dispose the arrangement.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        public static IStepDefinition Given(this string message, Action arrange, Action dispose)
        {
            return new StepDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(arrange, dispose))));
        }

        /// <summary>
        /// Defines an act to be performed in the scenario.
        /// </summary>
        /// <param name="message">A message describing the act.</param>
        /// <param name="act">The action that will perform the act.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        public static IStepDefinition When(this string message, Action act)
        {
            return new StepDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(act))));
        }

        /// <summary>
        /// Defines an assertion of an expected outcome of the scenario with all arrangements and actions isolated for this assertion.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        public static IStepDefinition ThenInIsolation(this string message, Action assert)
        {
            return new StepDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(assert), true)));
        }

        /// <summary>
        /// Defines an assertion of an expected outcome of the scenario with all arrangements and actions shared with other assertions.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="assert">The action which will perform the assertion.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        public static IStepDefinition Then(this string message, Action assert)
        {
            return new StepDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(assert))));
        }

        /// <summary>
        /// Defines a skipped assertion of an expected outcome for the scenario.
        /// </summary>
        /// <param name="message">A message describing the assertion.</param>
        /// <param name="assert">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        public static IStepDefinition ThenSkip(this string message, Action assert)
        {
            return new StepDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(assert), "Skipped for an unknown reason.")));
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The action which performs the step.</param>
        /// <param name="inIsolation">if set to <c>true</c> an isolated context will be created containing this step and a copy of all preceding steps.</param>
        /// <param name="skip">
        /// Marks the step so that it will not be executed and provides the reason.
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [CLSCompliant(false)]
        public static IStepDefinition _(this string message, Action step, bool inIsolation = false, string skip = null)
        {
            return new StepDefinition(CurrentThread.Scenario.Enqueue(new Step(message, DisposableFunctionFactory.Create(step), inIsolation, skip)));
        }
    }
}
