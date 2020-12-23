using System;
using System.Threading.Tasks;
using Xbehave.Sdk;

namespace Xbehave
{
    /// <summary>
    /// Provides extension methods for the definition of a step within a scenario.
    /// </summary>
    public static class IStepDefinitionExtensions
    {
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
            IStepBuilder stepBuilder = stepDefinition;
            stepBuilder.Teardown(action);
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
        public static IStepDefinition Teardown(this IStepDefinition stepDefinition, Action<IStepContext> action)
        {
            IStepBuilder stepBuilder = stepDefinition;
            stepBuilder.Teardown(action);
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
        public static IStepDefinition Teardown(this IStepDefinition stepDefinition, Func<Task> action)
        {
            IStepBuilder stepBuilder = stepDefinition;
            stepBuilder.Teardown(action);
            return stepDefinition;
        }
    }
}
