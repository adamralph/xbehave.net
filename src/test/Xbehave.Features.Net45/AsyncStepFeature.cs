// <copyright file="AsyncStepFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Features
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xbehave;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Sdk;

    public class AsyncStepFeature
    {
        [Scenario]
        public void AsyncStep(bool asyncStepHasCompleted)
        {
            "When an async step is executed"._(async () =>
            {
                await Task.Delay(500);
                asyncStepHasCompleted = true;
            });

            "Then it is completed before the next step is executed"._(() =>
                asyncStepHasCompleted.Should().BeTrue());
        }

        [Scenario]
        public void AllMethodsAreUsedAsync(int count)
        {
            "Given the count is 20".Given(async () =>
            {
                await Task.Yield();
                count = 20;
            });

            "When it is increased by one".When(async () =>
            {
                await Task.Yield();
                count++;
            });

            "And it is increased by two using the underscore method"._(async () =>
            {
                await Task.Yield();
                count += 2;
            });

            "And it is increased by two using the f method".f(async () =>
            {
                await Task.Yield();
                count += 2;
            });

            "Then it is 25"._(async () =>
            {
                await Task.Yield();
                count.Should().Be(25);
            });

            "And obviously it is greater than 10".And(async () =>
            {
                await Task.Yield();
                count.Should().BeGreaterThan(10);
            });

            "But evidently it is not 24".But(async () =>
            {
                await Task.Yield();
                count.Should().NotBe(24);
            });
        }

        [Scenario]
        public void MultipleAsyncSteps(int number)
        {
            "Given a number initialised as 2"._(async () =>
            {
                await Task.Delay(100);
                number = 2;
            });

            "When it is incremented in an asynchronous step"._(async () =>
            {
                await Task.Delay(100);
                number++;
            });

            "And it is incremented again in another asynchronous step"._(async () =>
            {
                await Task.Delay(100);
                number++;
            });

            "Then it is 4"._(() =>
                number.Should().Be(4));
        }

        [Scenario]
        public void AsyncStepDoesNotTimeout(bool asyncStepHasCompleted)
        {
            "When an asynchronous step with a timeout which does not timeout is executed"._(async () =>
            {
                await Task.Delay(500);
                asyncStepHasCompleted = true;
            })
            .WithTimeout(1000);

            "Then it has completed before the next step begins"._(() =>
                asyncStepHasCompleted.Should().BeTrue());
        }

        [Scenario]
        public void AsyncTaskStepThrowsException(Type feature, MethodResult[] results, Exception e)
        {
            "Given a feature with a scenario that throws an InvalidOperationException"._(() =>
                feature = typeof(AsyncTaskStepWhichThrowsException));

            "When the test runner runs the feature"._(() =>
                results = TestRunner.Run(feature).ToArray());

            "Then the result should be a failure"._(() =>
                results.Should().ContainItemsAssignableTo<FailedResult>());

            "And the exception type should be \"System.InvalidOperationException\"".And(() =>
                results.Cast<FailedResult>().Single().ExceptionType.Should().Be("System.InvalidOperationException"));
        }

        [Scenario]
        public void AsyncVoidStepThrowsException(Type feature, MethodResult[] results, Exception e)
        {
            "Given a feature with a scenario that throws an InvalidOperationException"._(() =>
                feature = typeof(AsyncVoidStepWhichThrowsException));

            "When the test runner runs the feature"._(() =>
                results = TestRunner.Run(feature).ToArray());

            "Then the result should be a failure"._(() =>
                results.Should().ContainItemsAssignableTo<FailedResult>());

            "And the exception type should be \"System.InvalidOperationException\""._(() =>
                results.Cast<FailedResult>().First().ExceptionType.Should().Be("System.InvalidOperationException"));
        }

        [Scenario]
        public void AsyncStepExceedsTimeout(Type feature, MethodResult[] results)
        {
            "Given a feature with a scenario with a single step which exceeds it's 1ms timeout"._(() =>
                feature = typeof(AsyncStepWhichExceedsTimeout));

            "When the test runner runs the feature"._(() =>
                results = TestRunner.Run(feature).ToArray());

            "Then there should be one result"._(() =>
                results.Count().Should().Be(1));

            "And the result should be a failure"._(() =>
                results.Should().ContainItemsAssignableTo<FailedResult>());

            "And the result message should be \"Test execution time exceeded: 1ms\""._(() =>
                results.Cast<FailedResult>().Should().OnlyContain(result => result.Message == "Test execution time exceeded: 1ms"));
        }

        [Scenario]
        public void ExecutingAnAsyncVoidStepUsingMethodGroupSyntax()
        {
            "When an async void method is executed in a step using method group syntax"._(
                (Action)AsyncVoidMethodType.AsyncVoidMethod);

            "Then the method has completed before the next step begins"._(() =>
                AsyncVoidMethodType.AsyncVoidMethodHasCompleted.Should().BeTrue());
        }

        private static class AsyncVoidMethodType
        {
            private static bool asyncVoidMethodHasCompleted;

            public static bool AsyncVoidMethodHasCompleted
            {
                get { return AsyncVoidMethodType.asyncVoidMethodHasCompleted; }
            }

            public static async void AsyncVoidMethod()
            {
                asyncVoidMethodHasCompleted = false;
                await Task.Delay(500);
                asyncVoidMethodHasCompleted = true;
            }
        }

        private class AsyncTaskStepWhichThrowsException
        {
            [Scenario]
            public void Scenario()
            {
                // disabling warning about async method not having await. it's intended
#pragma warning disable 1998
                "Given something"._(async () =>
                {
                    throw new InvalidOperationException();
                });
#pragma warning restore 1998
            }
        }

        private class AsyncVoidStepWhichThrowsException
        {
            [Scenario]
            public void Scenario()
            {
                "Given something"._(
                    (Action)this.Step);
            }

            private async void Step()
            {
                await Task.Yield();
                throw new InvalidOperationException();
            }
        }

        private class AsyncStepWhichExceedsTimeout
        {
            [Scenario]
            public void Scenario(bool set)
            {
                "Given something"._(async () =>
                    await Task.Delay(500)).WithTimeout(1);
            }
        }
    }
}
