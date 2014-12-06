// <copyright file="StringExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>
namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Fluent;

    /// <summary>
    /// Provides access to step definition methods.
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "f", Justification = "Fluent API")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "f", Justification = "Fluent API")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        public static IStep f(this string text, Action body)
        {
            return new Step(text, body);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "_", Justification = "Fluent API")]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Fluent API")]
        [CLSCompliant(false)]
        public static IStep _(this string text, Action body)
        {
            return new Step(text, body);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "f", Justification = "Fluent API")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "f", Justification = "Fluent API")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        public static IStep f(this string text, Action<IStepContext> body)
        {
            return new Step(text, body);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "_", Justification = "Fluent API")]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Fluent API")]
        [CLSCompliant(false)]
        public static IStep _(this string text, Action<IStepContext> body)
        {
            return new Step(text, body);
        }
    }
}
