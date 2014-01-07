namespace Xbehave.Features
{
    using FluentAssertions;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Sdk;

    public class AsyncStepFeature
    {
        private static readonly ManualResetEventSlim @Event = new ManualResetEventSlim();

        [Scenario]
        public void AsyncStepExecutesToCompletionBeforeNextStep(bool set)
        {
            "Given a boolean set to false"._(() => set = false);

            "When it is set to true inside an asynchronous step"._(async () =>
            {
                await Task.Delay(500);
                set = true;
            });

            "Then its value is true when the next step begins"._(() =>
            {
                set.Should().BeTrue();
            });
        }

        [Scenario]
        public void MultipleAsyncStepExecuteToCompletionBeforeNextStep(int first, int second, int third)
        {
            "Given three numbers initialized in 2"._(async () =>
            {
                await Task.Delay(100);
                first = 2;
                second = 2;
                third = 2;
            });

            "When all of them are incremented in an asynchronous step"._(async () =>
            {
                await Task.Delay(100);
                first++;
                second++;
                third++;
            });

            "And all of them are incremented in an asynchronous step again"._(async () =>
            {
                await Task.Delay(100);
                first++;
                second++;
                third++;
            });

            "Then all values are 4"._(() =>
            {
                first.Should().Be(4);
                first.Should().Be(4);
                first.Should().Be(4);
            });
        }

        [Scenario]
        public void AsyncStepThatExecutesBeforeTimeoutExpiresIsCompleted(bool set)
        {
            "Given a boolean set to false"._(() => set = false);

            "When it is set to true inside an asynchronous step"._(async () =>
            {
                await Task.Delay(500);
                set = true;
            }).WithTimeout(1000);

            "Then its value is true when the next step begins"._(() =>
            {
                set.Should().BeTrue();
            });
        }

        [Scenario]
        public static void StepExecutesTooSlowly()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a scenario with a single step which exceeds it's 1ms timeout"
                .Given(() => feature = typeof(StepTooSlow));

            "When the test runner runs the feature"
                .When(() =>
                {
                    @Event.Reset();
                    results = TestRunner.Run(feature).ToArray();
                })
                .Teardown(() => @Event.Set());

            "Then there should be one result"
                .Then(() => results.Count().Should().Be(1));

            "And the result should be a failure"
                .And(() => results.Should().ContainItemsAssignableTo<FailedResult>());

            "And the result message should be \"Test execution time exceeded: 1ms\""
                .And(() => results.Cast<FailedResult>().Should().OnlyContain(result => result.Message == "Test execution time exceeded: 1ms"));
        }

        private class StepTooSlow
        {
            [Scenario]
            public void Scenario(bool set)
            {
                "Given something"._(async () => await Task.Delay(500)).WithTimeout(1);
            }
        }

    }
}
