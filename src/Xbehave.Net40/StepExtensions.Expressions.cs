// <copyright file="IStepExtensions.Expressions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using Xbehave.Fluent;

    /// <summary>
    /// <see cref="IStep"/> extensions.
    /// </summary>
    public static partial class StepExtensions
    {
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "When", Justification = "By design.")]
        public static IStep When(this IStep stepDefinition, Expression<Action> body)
        {
            return Helper.AddStep("When", body);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Then", Justification = "By design.")]
        public static IStep Then(this IStep stepDefinition, Expression<Action> body)
        {
            return Helper.AddStep("Then", body);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "By design.")]
        public static IStep And(this IStep stepDefinition, Expression<Action> body)
        {
            return Helper.AddStep("And", body);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        public static IStep But(this IStep stepDefinition, Expression<Action> body)
        {
            return Helper.AddStep("But", body);
        }
    }
}
