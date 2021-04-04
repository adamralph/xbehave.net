using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xbehave.Sdk;
using Xbehave.Test.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Xbehave.Test
{
    // In order to prevent bugs due to incorrect code
    // As a developer
    // I want to run automated acceptance tests describing each feature of my product using scenarios
    public class ScenarioFeature : Feature
    {
        // NOTE (adamralph): a plain xunit fact to prove that plain scenarios work in 2.x
        [Fact]
        public void ScenarioWithTwoPassingStepsAndOneFailingStepYieldsTwoPassesAndOneFail()
        {
            // arrange
            var feature = typeof(FeatureWithAScenarioWithTwoPassingStepsAndOneFailingStep);

            // act
            var results = this.Run<ITestResultMessage>(feature);

            // assert
            Assert.Equal(3, results.Length);
            Assert.All(results.Take(2), result => Assert.IsAssignableFrom<ITestPassed>(result));
            Assert.All(results.Skip(2), result => Assert.IsAssignableFrom<ITestFailed>(result));
        }

        [Scenario]
        public void ScenarioWithThreeSteps(Type feature, IMessageSinkMessage[] messages, ITestResultMessage[] results)
        {
            "Given a feature with a scenario with three steps"
                .x(() => feature = typeof(FeatureWithAScenarioWithThreeSteps));

            "When I run the scenarios"
                .x(() => results = (messages = this.Run<IMessageSinkMessage>(feature))
                    .OfType<ITestResultMessage>().ToArray());

            "Then there should be three results"
                .x(() => Assert.Equal(3, results.Length));

            "And the first result should have a display name ending with 'Step 1'"
                .x(() => Assert.EndsWith("Step 1", results[0].Test.DisplayName));

            "And the second result should have a display name ending with 'Step 2'"
                .x(() => Assert.EndsWith("Step 2", results[1].Test.DisplayName));

            "And the third result should have a display name ending with 'Step 3'"
                .x(() => Assert.EndsWith("Step 3", results[2].Test.DisplayName));

            "And the messages should satisfy the xunit message contract"
                .x(() => Assert.Equal(
                    new[]
                    {
                        "TestCollectionStarting",
                        "TestClassStarting",
                        "TestMethodStarting",
                        "TestCaseStarting",
                        "TestStarting",
                        "TestPassed",
                        "TestFinished",
                        "TestStarting",
                        "TestPassed",
                        "TestFinished",
                        "TestStarting",
                        "TestPassed",
                        "TestFinished",
                        "TestCaseFinished",
                        "TestMethodFinished",
                        "TestClassFinished",
                        "TestCollectionFinished",
                    },
                    messages.Select(message => message.GetType().Name).SkipWhile(name => name == "TestAssemblyStarting").Take(17).ToArray()));
        }

        [Scenario]
        public void OrderingStepsByDisplayName(Type feature, ITestResultMessage[] results)
        {
            "Given ten steps named alphabetically backwards starting with 'z'"
                .x(() => feature = typeof(TenStepsNamedAlphabeticallyBackwardsStartingWithZ));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "And I sort the results by their display name"
                .x(() => results = results.OrderBy(result => result.Test.DisplayName).ToArray());

            "Then a concatenation of the last character of each result display names should be 'zyxwvutsrq'"
                .x(() => Assert.Equal("zyxwvutsrq", new string(results.Select(result => result.Test.DisplayName.Last()).ToArray())));
        }

        [Scenario]
        public void ScenarioWithTwoPassingStepsAndOneFailingStep(Type feature, ITestResultMessage[] results)
        {
            "Given a feature with a scenario with two passing steps and one failing step"
                .x(() => feature = typeof(FeatureWithAScenarioWithTwoPassingStepsAndOneFailingStep));

            "When I run the scenarios"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be three results"
                .x(() => Assert.Equal(3, results.Length));

            "And the first two results should be passes"
                .x(() => Assert.All(results.Take(2), result => Assert.IsAssignableFrom<ITestPassed>(result)));

            "And the third result should be a fail"
                .x(() => Assert.All(results.Skip(2), result => Assert.IsAssignableFrom<ITestFailed>(result)));
        }

        [Scenario]
        public void ScenarioBodyThrowsAnException(Type feature, Exception exception, ITestResultMessage[] results)
        {
            "Given a feature with a scenario body which throws an exception"
                .x(() => feature = typeof(FeatureWithAScenarioBodyWhichThrowsAnException));

            "When I run the scenarios"
                .x(() => exception = Record.Exception(() =>
                    results = this.Run<ITestResultMessage>(feature)));

            "Then no exception should be thrown"
                .x(() => Assert.Null(exception));

            "And the results should not be empty"
                .x(() => Assert.NotEmpty(results));

            "And each result should be a failure"
                .x(() => Assert.All(results, result => Assert.IsAssignableFrom<ITestFailed>(result)));
        }

        [Scenario]
        public void FeatureCannotBeConstructed(Type feature, Exception exception, ITestResultMessage[] results)
        {
            "Given a feature with a non-static scenario but no default constructor"
                .x(() => feature = typeof(FeatureWithANonStaticScenarioButNoDefaultConstructor));

            "When I run the scenarios"
                .x(() => exception = Record.Exception(() => results = this.Run<ITestResultMessage>(feature)));

            "Then no exception should be thrown"
                .x(() => Assert.Null(exception));

            "And the results should not be empty"
                .x(() => Assert.NotEmpty(results));

            "And each result should be a failure"
                .x(() => Assert.All(results, result => Assert.IsAssignableFrom<ITestFailed>(result)));
        }

        [Scenario]
        public void FeatureConstructionFails(Type feature, ITestFailed[] failures)
        {
            "Given a feature with a failing constructor"
                .x(() => feature = typeof(FeatureWithAFailingConstructor));

            "When I run the scenarios"
                .x(() => failures = this.Run<ITestFailed>(feature));

            "Then there should be one test failure"
                .x(() => Assert.Single(failures));
        }

        [Scenario]
        public void FailingStepThenPassingSteps(Type feature, ITestResultMessage[] results)
        {
            "Given a failing step and two passing steps named alphabetically backwards"
                .x(() => feature = typeof(AFailingStepAndTwoPassingStepsNamedAlphabeticallyBackwards));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "And I sort the results by their display name"
                .x(() => results = results.OrderBy(result => result.Test.DisplayName).ToArray());

            "Then the there should be three results"
                .x(() => Assert.Equal(3, results.Length));

            "Then the first result should be a failure"
                .x(() => Assert.IsAssignableFrom<ITestFailed>(results[0]));

            "And the second and third results should be skips"
                .x(() => Assert.All(results.Skip(1), result => Assert.IsAssignableFrom<ITestSkipped>(result)));

            "And the second result should refer to the second step"
                .x(() => Assert.Contains("Step y", results[1].Test.DisplayName));

            "And the third result should refer to the third step"
                .x(() => Assert.Contains("Step x", results[2].Test.DisplayName));

            "And the second and third result messages should indicate that the first step failed"
                .x(() => Assert.All(
                    results.Skip(1).Cast<ITestSkipped>(),
                    result =>
                    {
                        Assert.Contains("Failed to execute preceding step", result.Reason);
                        Assert.Contains("Step z", result.Reason);
                    }));
        }

        [Scenario]
        public void ScenarioWithNoSteps(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with no steps"
                .x(() => feature = typeof(FeatureWithAScenarioWithNoSteps));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one result"
                .x(() => Assert.Single(results));

            "And the result should be a pass"
                .x(() => Assert.IsAssignableFrom<ITestPassed>(results.Single()));
        }

        [Scenario]
        public void NullStepText() =>
            ((string)null)
                .x(() => { });

        [Scenario]
        public void NullStepBody() =>
            "Given a null body"
                .x((Action)null);

        [Scenario]
        public void NullContextualStepBody() =>
            "Given a null body"
                .x((Action<IStepContext>)null);

        [Scenario]
        public void NestedStep(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with a nested step"
                .x(() => feature = typeof(ScenarioWithANestedStep));

            "When I run the scenario"
                .x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one result"
                .x(() => Assert.Single(results));

            "And the result should be a fail"
                .x(() => Assert.IsAssignableFrom<ITestFailed>(results.Single()));
        }

        private class FeatureWithAScenarioWithThreeSteps
        {
            [Scenario]
            public void Scenario()
            {
                "Step 1"
                    .x(() => { });

                "Step 2"
                    .x(() => { });

                "Step 3"
                    .x(() => { });
            }
        }

        private class TenStepsNamedAlphabeticallyBackwardsStartingWithZ
        {
            [Scenario]
            public static void Scenario()
            {
                "z"
                    .x(() => { });

                "y"
                    .x(() => { });

                "x"
                    .x(() => { });

                "w"
                    .x(() => { });

                "v"
                    .x(() => { });

                "u"
                    .x(() => { });

                "t"
                    .x(() => { });

                "s"
                    .x(() => { });

                "r"
                    .x(() => { });

                "q"
                    .x(() => { });
            }
        }

        private class FeatureWithAScenarioWithTwoPassingStepsAndOneFailingStep
        {
            [Scenario]
            public static void Scenario()
            {
                var i = 0;

                "Given 1"
                    .x(() => i = 1);

                "When I add 1"
                    .x(() => i += 1);

                "Then I have 3"
                    .x(() => Assert.Equal(3, i));
            }
        }

        private class FeatureWithAScenarioBodyWhichThrowsAnException
        {
            [Scenario]
            public static void Scenario() => throw new InvalidOperationException();
        }

        private class AFailingStepAndTwoPassingStepsNamedAlphabeticallyBackwards
        {
            [Scenario]
            public static void Scenario()
            {
                "Step z"
                    .x(() => throw new NotImplementedException());

                "Step y"
                    .x(() => { });

                "Step x"
                    .x(() => { });
            }
        }

        private class FeatureWithANonStaticScenarioButNoDefaultConstructor
        {
            public FeatureWithANonStaticScenarioButNoDefaultConstructor(int _)
            {
            }

            [Scenario]
            public void Scenario() =>
                "Given something"
                    .x(() => { });
        }

        private class FeatureWithAFailingConstructor
        {
            public FeatureWithAFailingConstructor() => throw new InvalidOperationException();

            [Scenario]
            public void Scenario() =>
                "Given something"
                    .x(() => { });
        }

        private class FeatureWithAScenarioWithNoSteps
        {
            [Scenario]
            public void Scenario()
            {
            }
        }

        private class ScenarioWithANestedStep
        {
            [Scenario]
            public void Scenario() =>
                "Given something"
                    .x(() => "With something nested".x(() => { }));
        }
    }
}
