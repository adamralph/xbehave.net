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

        // used for async void method group test
        private static bool AsyncVoidSet = false;

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
        public void AsyncSupportForAllMethods(int calls)
        {
            calls = 0;

            "Given a variable initialized to 20".Given(async () => 
            {
                await Task.Yield();
                calls = 20;
            });

            "When it is increased by one".When(async () => 
            {
                await Task.Yield();
                calls++;
            });

            "And increased by four".And(async () => 
            {
                await Task.Yield();
                calls += 4;
            });

            "Then the resulting value should be 25".Then(async () =>
            {
                await Task.Yield();
                calls.Should().Be(25);
            });

            "And obviously is greater than 10".f(async () =>
            {
                await Task.Yield();
                calls.Should().BeGreaterThan(10);
            });

            "But evidently not 24".But(async () =>
            {
                await Task.Yield();
                calls.Should().NotBe(24);
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
        public void AsyncStepWithTaskReturnTypeThrowsException(Exception e)
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a scenario that throws an InvalidOperationException"._(() => feature = typeof(TaskReturningAsyncStepThrowsException));

            "When the test runner runs the feature"._(() =>
                {
                    results = TestRunner.Run(feature).ToArray();
                });

            "Then the result should be a failure"
                .And(() => results.Should().ContainItemsAssignableTo<FailedResult>());

            "And the exception type should be \"System.InvalidOperationException\""
                .And(() =>
                {
                    var failedResult = results.Cast<FailedResult>().Single();

                    failedResult.ExceptionType.Should().Be("System.InvalidOperationException");
                });
        }

        [Scenario]
        public void AsyncStepWithVoidReturnTypeThrowsException(Exception e)
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a feature with a scenario that throws an InvalidOperationException"._(() => feature = typeof(VoidReturningAsyncStepThrowsException));

            "When the test runner runs the feature"._(() =>
                {
                    results = TestRunner.Run(feature).ToArray();
                });

            "Then the result should be a failure"
                .And(() => results.Should().ContainItemsAssignableTo<FailedResult>());

            "And the exception type should be \"System.InvalidOperationException\""
                .And(() =>
                {
                    var failedResult = results.Cast<FailedResult>().First();

                    failedResult.ExceptionType.Should().Be("System.InvalidOperationException");
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

        [Scenario]
        public void AsyncStepExecutesToCompletionBeforeNextStepIfStepReturnsVoidNotTask(bool set)
        {
            "Given a boolean set to false"._(() => AsyncVoidSet = false);

            "When it is set to true inside an asynchronous step"._((Action)this.AsyncVoidStep);

            "Then its value is true when the next step begins"._(() =>
            {
                AsyncVoidSet.Should().BeTrue();
            });
        }

        private async void AsyncVoidStep()
        {
            await Task.Delay(500);
            AsyncVoidSet = true;
        }

        private class TaskReturningAsyncStepThrowsException
        {
            [Scenario]
            public void Scenario()
            {
                "Given something"._(async () => { throw new InvalidOperationException(); });
            }
        }

        private class VoidReturningAsyncStepThrowsException
        {
            [Scenario]
            public void Scenario()
            {
                "Given something"._((Action)Step);
            }

            private async void Step()
            {
                throw new InvalidOperationException();
            }
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
