// <copyright file="BackgroundFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Sdk;

    // In order to write less code
    // As a developer
    // I want to add background steps to all the scenarios in a feature
    public static class BackgroundFeature
    {
        private static readonly Queue<StepType> ExecutedStepTypes = new Queue<StepType>();

        private enum StepType
        {
            Background,
            Scenario,
        }

        [Scenario]
        public static void FeatureWithBackgroundSteps()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with three background steps and two scenarios with two steps each"
                .Given(() => feature = typeof(Feature));

            "When the test runner runs the feature"
                .When(() => results = TestRunner.Run(feature).ToArray());

            "Then the first three executed steps are background steps"
                .Then(() => ExecutedStepTypes.Dequeue(3).Should().OnlyContain(stepType => stepType == StepType.Background));

            "And the next two executed steps are scenario steps"
                .And(() => ExecutedStepTypes.Dequeue(2).Should().OnlyContain(stepType => stepType == StepType.Scenario));

            "And the next three executed steps are background steps"
                .And(() => ExecutedStepTypes.Dequeue(3).Should().OnlyContain(stepType => stepType == StepType.Background));

            "And the next two executed steps are scenario steps"
                .And(() => ExecutedStepTypes.Dequeue(2).Should().OnlyContain(stepType => stepType == StepType.Scenario));
        }

        private static class Feature
        {
            [Background]
            public static void Background()
            {
                "Given something"
                    .Given(() => ExecutedStepTypes.Enqueue(StepType.Background));

                "And something else"
                    .And(() => ExecutedStepTypes.Enqueue(StepType.Background));

                "And something else"
                    .And(() => ExecutedStepTypes.Enqueue(StepType.Background));
            }

            [Scenario]
            public static void Scenario1()
            {
                "Given something"
                    .Given(() => ExecutedStepTypes.Enqueue(StepType.Scenario));

                "And something else"
                    .And(() => ExecutedStepTypes.Enqueue(StepType.Scenario));
            }

            [Scenario]
            public static void Scenario2()
            {
                "Given something"
                    .Given(() => ExecutedStepTypes.Enqueue(StepType.Scenario));

                "And something else"
                    .And(() => ExecutedStepTypes.Enqueue(StepType.Scenario));
            }
        }
    }
}
