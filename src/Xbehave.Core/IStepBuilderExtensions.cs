namespace Xbehave
{
    using System;
    using System.Threading.Tasks;
    using Xbehave.Sdk;

    /// <summary>
    /// Provides extension methods for building steps.
    /// </summary>
    public static class IStepBuilderExtensions
    {
        /// <summary>
        /// Declares a teardown action (related to this step and/or previous steps) which will be executed
        /// after all steps in the current scenario have been executed.
        /// </summary>
        /// <param name="stepBuilder">The step builder.</param>
        /// <param name="action">The action.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        public static IStepBuilder Teardown(this IStepBuilder stepBuilder, Action action) =>
            action == null
                ? stepBuilder
                : stepBuilder.Teardown(context => action());

        /// <summary>
        /// Declares a teardown action (related to this step and/or previous steps) which will be executed
        /// after all steps in the current scenario have been executed.
        /// </summary>
        /// <param name="stepBuilder">The step builder.</param>
        /// <param name="action">The action.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        public static IStepBuilder Teardown(this IStepBuilder stepBuilder, Action<IStepContext> action) =>
            action == null
                ? stepBuilder
                : stepBuilder?.Teardown(context =>
                    {
                        action(context);
                        return Task.FromResult(0);
                    });

        /// <summary>
        /// Declares a teardown action (related to this step and/or previous steps) which will be executed
        /// after all steps in the current scenario have been executed.
        /// </summary>
        /// <param name="stepBuilder">The step builder.</param>
        /// <param name="action">The action.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        public static IStepBuilder Teardown(this IStepBuilder stepBuilder, Func<Task> action) =>
            action == null
                ? stepBuilder
                : stepBuilder?.Teardown(context => action());
    }
}
