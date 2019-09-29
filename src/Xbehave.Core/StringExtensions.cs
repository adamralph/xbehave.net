namespace Xbehave
{
    using System;
    using System.Threading.Tasks;
    using Xbehave.Sdk;

    /// <summary>
    /// Provides access to step definition methods.
    /// </summary>
    public static class StringExtensions
    {
#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        public static IStepBuilder x(this string text, Action body)
        {
            var stepDefinition = new StepDefinition
            {
                Text = text,
                Body = body == null
                    ? (Func<IStepContext, Task>)null
                    : c =>
                        {
                            body();
                            return Task.FromResult(0);
                        },
            };

            CurrentThread.Add(stepDefinition);
            return stepDefinition;
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        public static IStepBuilder x(this string text, Action<IStepContext> body)
        {
            var stepDefinition = new StepDefinition
            {
                Text = text,
                Body = body == null
                    ? (Func<IStepContext, Task>)null
                    : c =>
                        {
                            body(c);
                            return Task.FromResult(0);
                        },
            };

            CurrentThread.Add(stepDefinition);
            return stepDefinition;
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        public static IStepBuilder x(this string text, Func<Task> body)
        {
            var stepDefinition = new StepDefinition
            {
                Text = text,
                Body = body == null ? (Func<IStepContext, Task>)null : c => body(),
            };

            CurrentThread.Add(stepDefinition);
            return stepDefinition;
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        public static IStepBuilder x(this string text, Func<IStepContext, Task> body)
        {
            var stepDefinition = new StepDefinition { Text = text, Body = body, };
            CurrentThread.Add(stepDefinition);
            return stepDefinition;
        }
#pragma warning restore IDE1006 // Naming Styles
    }
}
