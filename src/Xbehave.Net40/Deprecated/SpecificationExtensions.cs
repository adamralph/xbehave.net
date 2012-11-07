// <copyright file="SpecificationExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Fluent;

    /// <summary>
    /// This member is deprecated.
    /// </summary>
    [Obsolete("Use StringExtensions instead.")]
    public static class SpecificationExtensions
    {
        /// <summary>
        /// This member is deprecated.
        /// </summary>
        /// <param name="message">A message describing the established context.</param>
        /// <param name="body">The action that will establish the context.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [Obsolete("Use Given() instead.")]
        public static IStep Context(this string message, Action body)
        {
            return message.Given(body);
        }

        /// <summary>
        /// This member is deprecated.
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
        /// This member is deprecated.
        /// </summary>
        /// <param name="message">A message describing the established context.</param>
        /// <param name="body">The action that will establish and return the context for this test.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [Obsolete("Use Given() instead.")]
        public static IStep ContextFixture(this string message, ContextDelegate body)
        {
            return message.GivenDisposable(body);
        }

        /// <summary>
        /// This member is deprecated.
        /// </summary>
        /// <param name="message">A message describing the action.</param>
        /// <param name="body">The action to perform.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [Obsolete("Use When() instead.")]
        public static IStep Do(this string message, Action body)
        {
            return message.When(body);
        }

        /// <summary>
        /// This member is deprecated.
        /// </summary>
        /// <param name="message">A message describing the expected result.</param>
        /// <param name="body">The action that will verify the expectation.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [Obsolete("Use Then().InIsolation() instead.")]
        public static IStep Assert(this string message, Action body)
        {
            return message.ThenInIsolation(body);
        }

        /// <summary>
        /// This member is deprecated.
        /// </summary>
        /// <param name="message">A message describing the expected result.</param>
        /// <param name="body">The action that will verify the expectation.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [Obsolete("Use Then() instead.")]
        public static IStep Observation(this string message, Action body)
        {
            return message.Then(body);
        }

        /// <summary>
        /// This member is deprecated.
        /// </summary>
        /// <param name="message">A message describing the expected result.</param>
        /// <param name="body">The action that will verify the expectation.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [Obsolete("Use Then().Skip() instead.")]
        public static IStep Todo(this string message, Action body)
        {
            return message.ThenSkip(body);
        }
    }
}