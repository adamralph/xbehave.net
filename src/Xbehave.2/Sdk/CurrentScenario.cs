// <copyright file="CurrentScenario.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class CurrentScenario
    {
        [ThreadStatic]
        private static bool addingBackgroundSteps;

        [ThreadStatic]
        private static List<Step> steps;

        public static bool AddingBackgroundSteps
        {
            get { return CurrentScenario.addingBackgroundSteps; }
            set { CurrentScenario.addingBackgroundSteps = value; }
        }

        private static List<Step> Steps
        {
            get { return steps ?? (steps = new List<Step>()); }
        }

        public static Step AddStep(string name, Action body)
        {
            var step = new Step(EmbellishStepName(name), body);
            Steps.Add(step);
            return step;
        }

        public static Step AddStep(string name, Func<Task> body)
        {
            var step = new Step(EmbellishStepName(name), body);
            Steps.Add(step);
            return step;
        }

        public static IEnumerable<Step> ExtractSteps()
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

        private static string EmbellishStepName(string name)
        {
            return addingBackgroundSteps ? "(Background) " + name : name;
        }
    }
}