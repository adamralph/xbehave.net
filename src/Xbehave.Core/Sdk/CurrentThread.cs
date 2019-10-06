// <copyright file="CurrentThread.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents the currently executing thread.
    /// </summary>
    public static class CurrentThread
    {
        [ThreadStatic]
        private static List<IStepDefinition> stepDefinitions;

        /// <summary>
        /// Causes the currently executing thread to enter a step definition context
        /// which allows step definitions to be added using <see cref="Add(IStepDefinition)"/>
        /// and retreived using <see cref="StepDefinitions"/>.
        /// </summary>
        /// <returns>
        /// An object which, when disposed, causes the currently executing thread to leave the step definition context.
        /// </returns>
        public static IDisposable EnterStepDefinitionContext()
        {
            stepDefinitions = new List<IStepDefinition>();

            return new StepDefinitionContext();
        }

        /// <summary>
        /// Add a step definition to the currently executing thread.
        /// </summary>
        /// <param name="item">The step definition.</param>
        /// <exception cref="InvalidOperationException">
        /// The currently executing thread is not in a step definition context.
        /// </exception>
        /// <remarks>
        /// Before this method is called, the thread must have entered a step definiton context
        /// with a call to <see cref="EnterStepDefinitionContext"/>.
        /// </remarks>
        public static void Add(IStepDefinition item)
        {
            if (stepDefinitions == null)
            {
                throw new InvalidOperationException("The currently executing thread is not in a step definition context.");
            }

            stepDefinitions.Add(item);
        }

        /// <summary>
        /// Gets the step definitions for the currently executing thread.
        /// </summary>
        /// <remarks>
        /// If the currently executing thread is not in a step definition context,
        /// the <see cref="IEnumerable{T}"/> will be empty.
        /// </remarks>
        public static IEnumerable<IStepDefinition> StepDefinitions =>
            stepDefinitions ?? Enumerable.Empty<IStepDefinition>();

        private sealed class StepDefinitionContext : IDisposable
        {
            public void Dispose() => stepDefinitions = null;
        }
    }
}
