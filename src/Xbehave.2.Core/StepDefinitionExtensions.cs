// <copyright file="StepDefinitionExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using Xbehave.Sdk;

    /// <summary>
    /// Provides extension methods for building step definitions.
    /// </summary>
    public static class StepDefinitionExtensions
    {
        /// <summary>
        /// Indicates that the step will not be executed.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="reason">The reason for not executing the step.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        /// <remarks>If the <paramref name="reason"/> is <c>null</c> then the step will not be skipped.</remarks>
        public static IStepDefinition Skip(this IStepDefinition stepDefinition, string reason)
        {
            Guard.AgainstNullArgument("stepDefinition", stepDefinition);

            stepDefinition.SkipReason = reason;
            return stepDefinition;
        }

        /// <summary>
        /// Declares a teardown action (related to this step and/or previous steps) which will be executed
        /// after all steps in the current scenario have been executed.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="action">The action.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Teardown(this IStepDefinition stepDefinition, Action action)
        {
            Guard.AgainstNullArgument("stepDefinition", stepDefinition);

            stepDefinition.Teardowns.Add(action);
            return stepDefinition;
        }

        /// <summary>
        /// Defines the behavior of remaining steps if this step fails.
        /// </summary>
        /// <param name="stepDefinition">The step definition.</param>
        /// <param name="remainingSteps">The behavior of remaining steps.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition OnFailure(this IStepDefinition stepDefinition, RemainingSteps remainingSteps)
        {
            Guard.AgainstNullArgument("stepDefinition", stepDefinition);

            stepDefinition.OnFailure = remainingSteps;
            return stepDefinition;
        }
    }
}
