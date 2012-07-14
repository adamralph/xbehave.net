// <copyright file="StepDefinitionExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using Xbehave.Fluent;

    /// <summary>
    /// Extensions for the definition of a scenario step.
    /// </summary>
    public static partial class StepDefinitionExtensions
    {
        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition When(this IStepDefinition stepDefinition, string text, Action body)
        {
            return StepDefinition.Create("When", text, body);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Then(this IStepDefinition stepDefinition, string text, Action body)
        {
            return StepDefinition.Create("Then", text, body);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition And(this IStepDefinition stepDefinition, string text, Action body)
        {
            return StepDefinition.Create("And", text, body);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition But(this IStepDefinition stepDefinition, string text, Action body)
        {
            return StepDefinition.Create("But", text, body);
        }
    }
}
