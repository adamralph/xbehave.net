namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the currently executing thread.
    /// </summary>
    public static class CurrentThread
    {
        [ThreadStatic]
        private static List<IStepDefinition> stepDefinitions;

        /// <summary>
        /// Gets the step definitions for the currently executing thread.
        /// </summary>
        public static ICollection<IStepDefinition> StepDefinitions =>
            stepDefinitions ?? (stepDefinitions = new List<IStepDefinition>());
    }
}
