// <copyright file="FeatureFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit;
    using Xunit.Sdk;

    // In order to prevent bugs due to incorrect code
    // As a developer
    // I want to run automated acceptance tests describing each feature of my product
    public static class FeatureFeature
    {
        [Scenario]
        public static void RunningAFeatureWithAScenarioWithInvalidExamples()
        {
            var feature = default(Type);
            var exception = default(Exception);
            var results = default(MethodResult[]);

            "Given a feature with scenarios with invalid examples"
                .Given(() => feature = typeof(FeatureWithScenariosWithInvalidExamples));

            "When the test runner runs the feature"
                .When(() => exception = Record.Exception(() => results = TestRunner.Run(feature).ToArray()));

            "Then no exception should be thrown"
                .Then(() => exception.Should().BeNull());

            "And the results should not be empty"
                .And(() => results.Should().NotBeEmpty());

            "And the results should be failures"
                .And(() => results.Should().ContainItemsAssignableTo<FailedResult>());
        }

        [Scenario]
        public static void RunningAFeatureWithAScenarioDefinitionWhichThrowsAnException()
        {
            var feature = default(Type);
            var exception = default(Exception);
            var results = default(MethodResult[]);

            "Given a feature with a scenario definition which throws an exception"
                .Given(() => feature = typeof(FeatureWithAScenarioDefinitionWhichThrowsAnException));

            "When the test runner runs the feature"
                .When(() => exception = Record.Exception(() => results = TestRunner.Run(feature).ToArray()));

            "Then no exception should be thrown"
                .Then(() => exception.Should().BeNull());

            "And the results should not be empty"
                .And(() => results.Should().NotBeEmpty());

            "And the results should be failures"
                .And(() => results.Should().ContainItemsAssignableTo<FailedResult>());
        }

        private static class FeatureWithScenariosWithInvalidExamples
        {
            [Scenario]
            public static void Scenario1(int i)
            {
            }

            [Scenario]
            [Example("a")]
            public static void Scenario2(int i)
            {
            }

            [Scenario]
            [Example(1, 2)]
            public static void Scenario3(int i)
            {
            }
            
            [Scenario]
            [Example(1)]
            public static void Scenario4(int i, int j)
            {
            }
        }

        private static class FeatureWithAScenarioDefinitionWhichThrowsAnException
        {
            [Scenario]
            public static void Scenario()
            {
                throw new InvalidOperationException();
            }
        }
    }
}
