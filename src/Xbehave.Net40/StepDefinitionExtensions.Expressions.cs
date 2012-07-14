// <copyright file="StepDefinitionExtensions.Expressions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using Xbehave.Fluent;

    /// <summary>
    /// Extensions for the definition of a scenario step.
    /// </summary>
    public static partial class StepDefinitionExtensions
    {
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "When", Justification = "By design.")]
        public static IStepDefinition When(this IStepDefinition stepDefinition, Expression<Action> body)
        {
            return StepDefinition.Create("When", body);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Then", Justification = "By design.")]
        public static IStepDefinition Then(this IStepDefinition stepDefinition, Expression<Action> body)
        {
            return StepDefinition.Create("Then", body);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "By design.")]
        public static IStepDefinition And(this IStepDefinition stepDefinition, Expression<Action> body)
        {
            return StepDefinition.Create("And", body);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition But(this IStepDefinition stepDefinition, Expression<Action> body)
        {
            return StepDefinition.Create("But", body);
        }
    }
}
