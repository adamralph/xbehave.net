// <copyright file="ThreadStaticStepHub.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Used for creating instances of <see cref="StepDefinition"/> during the execution of background and scenario methods
    /// which can be subsequently retrieved on the same thread.
    /// </summary>
    /// <remarks>
    /// Initially creates scenario steps,
    /// but can be instructed to create background steps via the <see cref="CreateBackgroundSteps"/> method.
    /// </remarks>
    public static class ThreadStaticStepHub
    {
        [ThreadStatic]
        private static bool creatingBackgroundSteps;

        [ThreadStatic]
        private static List<IStepDefinition> steps;

        private static List<IStepDefinition> Steps
        {
            get { return steps ?? (steps = new List<IStepDefinition>()); }
        }

        /// <summary>
        /// Instructs the <see cref="ThreadStaticStepHub"/> to create subsequent steps as background steps
        /// until the returned object is disposed.
        /// </summary>
        /// <returns>
        /// An <see cref="IDisposable"/> which, when disposed,
        /// instructs the <see cref="ThreadStaticStepHub"/> to create subsequent steps as scenario steps.
        /// </returns>
        public static IDisposable CreateBackgroundSteps()
        {
            return new BackgroundStepCreation();
        }

        /// <summary>
        /// Creates a <see cref="StepDefinition"/> with specified <paramref name="text"/> and <paramref name="body"/>
        /// and adds it to the <see cref="ThreadStaticStepHub"/>.
        /// </summary>
        /// <param name="text">The natural language associated with step.</param>
        /// <param name="body">The body of the step.</param>
        /// <returns>A <see cref="IStepDefinition"/>.</returns>
        public static IStepDefinition CreateAndAdd(string text, Func<IStepContext, Task> body)
        {
            var step = new StepDefinition
            {
                Body = body,
                IsBackgroundStep = creatingBackgroundSteps,
                Text = text,
            };

            Steps.Add(step);
            return step;
        }

        /// <summary>
        /// Removes all the <see cref="IStepDefinition"/> instances from the <see cref="ThreadStaticStepHub"/>
        /// which were created on the current thread.
        /// </summary>
        /// <returns>A <see cref="IList{T}"/> of <see cref="StepDefinition"/> instances.</returns>
        public static IList<IStepDefinition> RemoveAll()
        {
            try
            {
                return Steps;
            }
            finally
            {
                steps = null;
            }
        }

        private sealed class BackgroundStepCreation : IDisposable
        {
            public BackgroundStepCreation()
            {
                creatingBackgroundSteps = true;
            }

            public void Dispose()
            {
                creatingBackgroundSteps = false;
            }
        }
    }
}
