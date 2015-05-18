// <copyright file="StepDefinitionFilterFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if V2
namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Sdk;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Abstractions;

    public class StepDefinitionFilterFeature : Feature
    {
        [Scenario]
        public void SkipAll(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario marked with SkipAll"
                .f(() => feature = typeof(ScenarioWithSkipAll));

            "When I run the scenario"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then the steps are skipped"
                .f(() => results.Should().NotBeEmpty().And.ContainItemsAssignableTo<ITestSkipped>());
        }

        [Scenario]
        public void ContinueAfterThen(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario marked with ContinueAfterThen"
                .f(() => feature = typeof(ScenarioWithContinueAfterThen));

            "When I run the scenario"
                .f(() => results = this.Run<ITestResultMessage>(feature));

            "Then there are four results"
                .f(() => results.Length.Should().Be(4));

            "Then the first two steps pass"
                .f(() => results.Take(2).Should().ContainItemsAssignableTo<ITestPassed>());

            "And the third step fails"
                .f(() => results.Skip(2).Take(1).Should().ContainItemsAssignableTo<ITestFailed>());

            "And the fourth step passes"
                .f(() => results.Skip(3).Take(1).Should().ContainItemsAssignableTo<ITestPassed>());
        }

        private sealed class SkipAllAttribute : Attribute, IFilter<IStepDefinition>
        {
            public IEnumerable<IStepDefinition> Filter(IEnumerable<IStepDefinition> steps)
            {
                return steps.Select(step => step.Skip("test"));
            }
        }

        private class ScenarioWithSkipAll
        {
            [Scenario]
            [SkipAll]
            public void Scenario()
            {
                "Given something"
                    .f(() => { });

                "When something"
                    .f(() => { });

                "Then something"
                    .f(() => { });
            }
        }

        private sealed class ContinueAfterThenAttribute : Attribute, IFilter<IStepDefinition>
        {
            public IEnumerable<IStepDefinition> Filter(IEnumerable<IStepDefinition> steps)
            {
                var then = false;
                return steps.Select(step => step.OnFailure(
                    then || (then = step.Text.StartsWith("Then ", StringComparison.OrdinalIgnoreCase))
                    ? RemainingSteps.Run
                    : RemainingSteps.Skip));
            }
        }

        private class ScenarioWithContinueAfterThen
        {
            [Scenario]
            [ContinueAfterThen]
            public void Scenario()
            {
                "Given something"
                    .f(() => { });

                "When something"
                    .f(() => { });

                "Then something"
                    .f(() => { throw new InvalidOperationException(); });

                "And something"
                    .f(() => { });
            }
        }
    }
}
#endif
