// <copyright file="ActionExtensionsFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Test.Unit.Infrastructure
{
    using System;
    using FakeItEasy;
    using FluentAssertions;
    using Xbehave.Sdk.Infrastructure;
    using Xunit;

    public static class ActionExtensionsFeature
    {
        [Scenario]
        public static void InvokingManyActions()
        {
            var firstAction = default(Action);
            var secondAction = default(Action);

            "Given an action"
                .Given(() => firstAction = A.Fake<Action>());

            "And another action"
                .And(() => secondAction = A.Fake<Action>());

            "When invoking all the actions"
                .When(() => new[] { firstAction, secondAction }.InvokeAll());

            "Then the first action is invoked"
                .Then(() => A.CallTo(() => firstAction.Invoke()).MustHaveHappened(Repeated.Exactly.Once));

            "And the second action is invoked"
                .And(() => A.CallTo(() => secondAction.Invoke()).MustHaveHappened(Repeated.Exactly.Once));
        }

        [Scenario]
        public static void InvokingManyBadActions()
        {
            var firstException = default(Exception);
            var firstAction = default(Action);
            var secondException = default(Exception);
            var secondAction = default(Action);
            var thrownException = default(Exception);

            "Given an exception"
                .Given(() => firstException = new Exception("Exception 1"));

            "And an action which throws the exception"
                .And(() =>
                {
                    firstAction = A.Fake<Action>();
                    A.CallTo(() => firstAction.Invoke()).Throws(firstException);
                });

            "And a second exception"
                .And(() => secondException = new Exception("Exception 2"));

            "And a second action which throws the second exception"
                .And(() =>
                {
                    secondAction = A.Fake<Action>();
                    A.CallTo(() => secondAction.Invoke()).Throws(secondException);
                });

            "When invoking all the actions"
                .When(() => thrownException = Record.Exception(() => new[] { firstAction, secondAction }.InvokeAll()));

            "Then the thrown exception should be the second exception"
                .Then(() => thrownException.Should().Be(secondException));
        }
    }
}
