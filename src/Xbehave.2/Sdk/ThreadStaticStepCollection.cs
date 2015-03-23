// <copyright file="ThreadStaticStepCollection.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Used for collecting instances of <see cref="Step"/> during the execution of a scenario method
    /// which can subsequently be retrieved on the same thread.
    /// </summary>
    public static class ThreadStaticStepCollection
    {
        [ThreadStatic]
        private static bool addingBackgroundSteps;

        [ThreadStatic]
        private static List<Step> steps;

        private static List<Step> Steps
        {
            get { return steps ?? (steps = new List<Step>()); }
        }

        /// <summary>
        /// Instructs the <see cref="ThreadStaticStepCollection"/>
        /// that subsequently added steps are those defined in background methods.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> which, when disposed,
        /// instructs the <see cref="ThreadStaticStepCollection"/>
        /// that subsequently added steps are those defined in scenario methods.
        /// </returns>
        public static IDisposable ExpectBackgroundSteps()
        {
            return new BackgroundStepAddition();
        }

        /// <summary>
        /// Adds an instance of <see cref="Step"/> with specified <paramref name="text"/> and <paramref name="body"/>.
        /// </summary>
        /// <param name="text">The natural language associated with step.</param>
        /// <param name="body">The body of the step.</param>
        /// <returns>An instance of <see cref="Step"/>.</returns>
        public static Step Add(string text, Action body)
        {
            var step = new Step(EmbellishStepText(text), body);
            Steps.Add(step);
            return step;
        }

        /// <summary>
        /// Adds an instance of <see cref="Step"/> with specified <paramref name="text"/> and <paramref name="body"/>.
        /// </summary>
        /// <param name="text">The natural language associated with step.</param>
        /// <param name="body">The body of the step.</param>
        /// <returns>An instance of <see cref="Step"/>.</returns>
        public static Step Add(string text, Func<Task> body)
        {
            var step = new Step(EmbellishStepText(text), body);
            Steps.Add(step);
            return step;
        }

        /// <summary>
        /// Takes all the instances of <see cref="Step"/> from the <see cref="ThreadStaticStepCollection"/>,
        /// leaving it empty.
        /// </summary>
        /// <returns>A <see cref="IList{T}"/> of <see cref="Step"/> instances.</returns>
        public static IList<Step> TakeAll()
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

        private static string EmbellishStepText(string text)
        {
            return addingBackgroundSteps ? "(Background) " + text : text;
        }

        private sealed class BackgroundStepAddition : IDisposable
        {
            public BackgroundStepAddition()
            {
                addingBackgroundSteps = true;
            }

            public void Dispose()
            {
                addingBackgroundSteps = false;
            }
        }
    }
}
