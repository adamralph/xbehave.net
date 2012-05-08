// <copyright file="SpecificationExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Sdk.Fluent;

    /// <summary>
    /// Provides extensions for a fluent specification syntax
    /// </summary>
    [Obsolete("Use StringExtensions instead.")]
    public static class SpecificationExtensions
    {
        /// <summary>
        /// Records a context setup for this specification.
        /// </summary>
        /// <param name="message">A message describing the established context.</param>
        /// <param name="body">The action that will establish the context.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use Given() instead.")]
        public static IStepDefinition Context(this string message, Action body)
        {
            return message.Given(body);
        }

        /// <summary>
        /// Trap for using contexts implementing IDisposable with the wrong overload.
        /// </summary>
        /// <param name="message">A message describing the established context.</param>
        /// <param name="body">The action that will establish and return the context for this test.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "message", Justification = "The member is deprecated and will be removed.")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "body", Justification = "The member is deprecated and will be removed.")]
        [Obsolete("Use Given() instead.")]
        public static void Context(this string message, ContextDelegate body)
        {
            throw new InvalidOperationException("Use Given() instead.");
        }

        /// <summary>
        /// Records a disposable context for this specification. The context lifecycle will be managed by Xbehave.
        /// </summary>
        /// <param name="message">A message describing the established context.</param>
        /// <param name="body">The action that will establish and return the context for this test.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use Given() instead.")]
        public static IStepDefinition ContextFixture(this string message, ContextDelegate body)
        {
            return message.GivenDisposable(body);
        }

        /// <summary>
        /// Records an action to be performed on the context for this specification.
        /// </summary>
        /// <param name="message">A message describing the action.</param>
        /// <param name="body">The action to perform.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use When() instead.")]
        public static IStepDefinition Do(this string message, Action body)
        {
            return message.When(body);
        }

        /// <summary>
        /// Records an assertion for this specification.
        /// Each assertion is executed on an isolated context.
        /// </summary>
        /// <param name="message">A message describing the expected result.</param>
        /// <param name="body">The action that will verify the expectation.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use ThenInIsolation() instead.")]
        public static IStepDefinition Assert(this string message, Action body)
        {
            return message.ThenInIsolation(body);
        }

        /// <summary>
        /// Records an observation for this specification.
        /// All observations are executed on the same context.
        /// </summary>
        /// <param name="message">A message describing the expected result.</param>
        /// <param name="body">The action that will verify the expectation.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use Then() instead.")]
        public static IStepDefinition Observation(this string message, Action body)
        {
            return message.Then(body);
        }

        /// <summary>
        /// Records a skipped assertion for this specification.
        /// </summary>
        /// <param name="message">A message describing the expected result.</param>
        /// <param name="body">The action that will verify the expectation.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use ThenSkip() instead.")]
        public static IStepDefinition Todo(this string message, Action body)
        {
            return message.ThenSkip(body);
        }
    }
}