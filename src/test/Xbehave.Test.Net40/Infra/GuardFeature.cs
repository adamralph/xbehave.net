// <copyright file="GuardFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Infra
{
    using System;
    using FakeItEasy;
    using FluentAssertions;
    using Xbehave.Infra;
    using Xunit;

    public static class GuardFeature
    {
        [Scenario]
        public static void NullArgument()
        {
            var parameterName = default(string);
            var nullArgument = default(object);
            var exception = default(Exception);

            "Given a parameter name"
                .Given(() => parameterName = "foo");

            "And a null argument"
                .And(() => nullArgument = null);

            "When guarding against a null argument"
                .When(() => exception = Record.Exception(() => Guard.AgainstNullArgument(parameterName, nullArgument)));

            "Then the exception should be an argument null exception"
                .Then(() => exception.Should().BeOfType<ArgumentNullException>());

            "And the exception message should contain the parameter name and \"null\""
                .And(() => exception.Message.Should().Contain(parameterName).And.Contain("null"));

            "And the exception parameter name should be the parameter name"
                .And(() => exception.As<ArgumentException>().ParamName.Should().Be(parameterName));
        }

        [Scenario]
        public static void NullArgumentProperty()
        {
            var parameterName = default(string);
            var propertyName = default(string);
            var argument = default(IDummyDefinition);
            var exception = default(Exception);

            "Given a parameter name"
                .Given(() => parameterName = "foo");

            "And a property name"
                .And(() => propertyName = "Bar");

            "And an argument with a null property"
                .And(() =>
                {
                    argument = A.Fake<IDummyDefinition>();
                    A.CallTo(() => argument.ForType).Returns(null);
                });

            "When guarding against a null argument property"
                .When(() => exception = Record.Exception(() => Guard.AgainstNullArgumentProperty(parameterName, propertyName, argument.ForType)));

            "Then the exception should be an argument exception"
                .Then(() => exception.Should().BeOfType<ArgumentException>());

            "And the exception message should contain the parameter name, the property name and \"null\""
                .And(() => exception.Message.Should().Contain(parameterName).And.Contain(propertyName).And.Contain("null"));

            "And the exception parameter name should be the parameter name"
                .And(() => exception.As<ArgumentException>().ParamName.Should().Be(parameterName));
        }
    }
}
