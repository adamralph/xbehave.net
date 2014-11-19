// <copyright file="AsyncStepFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if !V2
namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xbehave;
    using Xbehave.Test.Acceptance.Infrastructure;

    public static class AsyncStepFeature
    {
        [Scenario]
        public static void AsyncStep(bool asyncStepHasCompleted)
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
        public static void AllMethodsAreUsedAsync(int count)
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
        public static void MultipleAsyncSteps(int number)
        {
            "Given a number initialized as 2"._(async () =>
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
        public static void AsyncStepDoesNotTimeout(bool asyncStepHasCompleted)
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
        public static void AsyncTaskStepThrowsException(Type feature, Result[] results)
        {
            "Given a feature with a scenario that throws an invalid operation exception"._(() =>
                feature = typeof(AsyncTaskStepWhichThrowsException));

            "When I run the scenarios"._(() =>
                results = feature.RunScenarios());

            "Then the result should be a failure"._(() =>
                results.Should().ContainItemsAssignableTo<Fail>());

            "And the exception should be an invalid operation exception".And(() =>
                results.Cast<Fail>().Single().ExceptionType.Should().Be("System.InvalidOperationException"));
        }

        [Scenario]
        public static void AsyncTaskStepThrowsExceptionWithinTimeout(Type feature, Result[] results)
        {
            "Given a feature with a scenario that throws an invalid operation exception within its timeout"._(() =>
                feature = typeof(AsyncTaskStepWhichThrowsExceptionWithinTimeout));

            "When I run the scenarios"._(() =>
                results = feature.RunScenarios());

            "Then the result should be a failure"._(() =>
                results.Should().ContainItemsAssignableTo<Fail>());

            "And the exception should be an invalid operation exception".And(() =>
                results.Cast<Fail>().Single().ExceptionType.Should().Be("System.InvalidOperationException"));
        }

        [Scenario]
        public static void AsyncVoidStepThrowsException(Type feature, Result[] results)
        {
            "Given a feature with a scenario that throws an invalid operation exception"._(() =>
                feature = typeof(AsyncVoidStepWhichThrowsException));

            "When I run the scenarios"._(() =>
                results = feature.RunScenarios());

            "Then the result should be a failure"._(() =>
                results.Should().ContainItemsAssignableTo<Fail>());

            "And the exception should be an invalid operation exception".And(() =>
                results.Cast<Fail>().First().ExceptionType.Should().Be("System.InvalidOperationException"));
        }

        [Scenario]
        public static void AsyncStepExceedsTimeout(Type feature, Result[] results)
        {
            "Given a feature with a scenario with a single step which exceeds it's 1ms timeout"._(() =>
                feature = typeof(AsyncStepWhichExceedsTimeout));

            "When I run the scenarios"._(() =>
                results = feature.RunScenarios());

            "Then there should be one result"._(() =>
                results.Count().Should().Be(1));

            "And the result should be a failure"._(() =>
                results.Should().ContainItemsAssignableTo<Fail>());

            "And the result message should be \"Test execution time exceeded: 1ms\""._(() =>
                results.Cast<Fail>().Should().OnlyContain(result => result.Message == "Test execution time exceeded: 1ms"));
        }

        [Scenario]
        public static void ExecutingAnAsyncVoidStepUsingMethodGroupSyntax()
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
                get { return asyncVoidMethodHasCompleted; }
            }

            public static async void AsyncVoidMethod()
            {
                asyncVoidMethodHasCompleted = false;
                await Task.Delay(500);
                asyncVoidMethodHasCompleted = true;
            }
        }

        private static class AsyncTaskStepWhichThrowsException
        {
            [Scenario]
            public static void Scenario()
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

        private static class AsyncTaskStepWhichThrowsExceptionWithinTimeout
        {
            [Scenario]
            public static void Scenario()
            {
                // disabling warning about async method not having await. it's intended
#pragma warning disable 1998
                "Given something"._(async () =>
                {
                    throw new InvalidOperationException();
                })
                .WithTimeout(1000);
#pragma warning restore 1998
            }
        }

        private static class AsyncVoidStepWhichThrowsException
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something"._(
                    (Action)Step);
            }

            private static async void Step()
            {
                await Task.Yield();
                throw new InvalidOperationException();
            }
        }

        private static class AsyncStepWhichExceedsTimeout
        {
            [Scenario]
            public static void Scenario(bool set)
            {
                "Given something"._(async () =>
                    await Task.Delay(500)).WithTimeout(1);
            }
        }
    }
}
#endif