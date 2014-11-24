// <copyright file="CurrentScenario.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    public static class CurrentScenario
    {
        [ThreadStatic]
        private static bool addingBackgroundSteps;

        [ThreadStatic]
        private static List<Step> steps;

        [ThreadStatic]
        private static List<Action> teardowns;

        public static bool AddingBackgroundSteps
        {
            get { return CurrentScenario.addingBackgroundSteps; }
            set { CurrentScenario.addingBackgroundSteps = value; }
        }

        private static List<Step> Steps
        {
            get { return steps ?? (steps = new List<Step>()); }
        }

        private static List<Action> Teardowns
        {
            get { return teardowns ?? (teardowns = new List<Action>()); }
        }

        public static Step AddStep(string name, Action body)
        {
            var step = new Step(EmbellishStepName(name), body.GetMethodInfo());
            Steps.Add(step);
            return step;
        }

        public static Step AddStep(string name, Func<Task> body)
        {
            var step = new Step(EmbellishStepName(name), body.GetMethodInfo());
            Steps.Add(step);
            return step;
        }

        public static void AddTeardown(Action teardown)
        {
            if (teardown != null)
            {
                Teardowns.Add(teardown);
            }
        }

        public static IEnumerable<Action> ExtractTeardowns()
        {
            try
            {
                return Teardowns;
            }
            finally
            {
                teardowns = null;
            }
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