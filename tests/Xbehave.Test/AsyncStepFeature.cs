// <copyright file="AsyncStepFeature.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Xbehave;
    using Xbehave.Test.Infrastructure;
    using Xunit.Abstractions;

    public class AsyncStepFeature : Feature
    {
        [Scenario]
        public void AsyncStep(bool asyncStepHasCompleted)
        {
            "When an async step is executed".x(async () =>
            {
                await Task.Delay(500);
                asyncStepHasCompleted = true;
            });

            "Then it is completed before the next step is executed".x(() =>
                asyncStepHasCompleted.Should().BeTrue());
        }

        [Scenario]
        public void AllMethodsAreUsedAsync(int count)
        {
            "Given the count is 20".x(async () =>
            {
                await Task.Yield();
                count = 20;
            });

            "When it is increased by one".x(async () =>
            {
                await Task.Yield();
                count++;
            });

            "And it is increased by two using the underscore method".x(async () =>
            {
                await Task.Yield();
                count += 2;
            });

            "And it is increased by two using the f method".x(async () =>
            {
                await Task.Yield();
                count += 2;
            });

            "Then it is 25".x(async () =>
            {
                await Task.Yield();
                count.Should().Be(25);
            });

            "And obviously it is greater than 10".x(async () =>
            {
                await Task.Yield();
                count.Should().BeGreaterThan(10);
            });

            "But evidently it is not 24".x(async () =>
            {
                await Task.Yield();
                count.Should().NotBe(24);
            });
        }

        [Scenario]
        public void MultipleAsyncSteps(int number)
        {
            "Given a number initialized as 2".x(async () =>
            {
                await Task.Delay(100);
                number = 2;
            });

            "When it is incremented in an asynchronous step".x(async () =>
            {
                await Task.Delay(100);
                number++;
            });

            "And it is incremented again in another asynchronous step".x(async () =>
            {
                await Task.Delay(100);
                number++;
            });

            "Then it is 4".x(() =>
                number.Should().Be(4));
        }

        [Scenario]
        public void AsyncTaskStepThrowsException(Type feature, ITestResultMessage[] results)
        {
            "Given a feature with a scenario that throws an invalid operation exception".x(() =>
                feature = typeof(AsyncTaskStepWhichThrowsException));

            "When I run the scenarios".x(() =>
                results = this.Run<ITestResultMessage>(feature));

            "Then the result should be a failure".x(() =>
                results.Should().ContainItemsAssignableTo<ITestFailed>());

            "And the exception should be an invalid operation exception".x(() =>
                results.Cast<ITestFailed>().Single().ExceptionTypes.Single().Should().Be("System.InvalidOperationException"));
        }

        [Scenario]
        public void AsyncVoidStepThrowsException(Type feature, ITestResultMessage[] results)
        {
            "Given a feature with a scenario that throws an invalid operation exception".x(() =>
                feature = typeof(AsyncVoidStepWhichThrowsException));

            "When I run the scenarios".x(() =>
                results = this.Run<ITestResultMessage>(feature));

            "Then the result should be a failure".x(() =>
                results.Should().ContainItemsAssignableTo<ITestFailed>());

            "And the exception should be an invalid operation exception".x(() =>
                results.Cast<ITestFailed>().First().ExceptionTypes.Single().Should().Be("System.InvalidOperationException"));
        }

        [Scenario]
        public void ExecutingAnAsyncVoidStepUsingMethodGroupSyntax()
        {
            "When an async void method is executed in a step using method group syntax".x(
                (Action)AsyncVoidMethodType.AsyncVoidMethod);

            "Then the method has completed before the next step begins".x(() =>
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
                "Given something".x(async () =>
                {
                    throw new InvalidOperationException();
                });
#pragma warning restore 1998
            }
        }

        private static class AsyncVoidStepWhichThrowsException
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something".x(
                    (Action)Step);
            }

            private static async void Step()
            {
                await Task.Yield();
                throw new InvalidOperationException();
            }
        }
    }
}
