// <copyright file="ArgumentSpecifications.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Specifications
{
    using System;
    using FluentAssertions;
    using Xbehave;

    public class ArgumentSpecifications
    {
        [Scenario]
        public static void SpecificValue(object value, Argument argument)
        {
            "Given a value"
                .Given(() => value = new object());

            "When constructing an argument using the value"
                .When(() => argument = new Argument(value));

            "Then the argument should not be a generated default"
                .Then(() => argument.IsGeneratedDefault.Should().BeFalse());

            "And the argument value should be the given value"
                .And(() => argument.Value.Should().Be(value));
        }

        [Scenario]
        [Example(typeof(int), 0)]
        [Example(typeof(object), null)]
        public static void GeneratedDefaultValue<TValue>(Type type, TValue expectedValue, Argument argument)
        {
            "Given the type {0}"
                .Given(() => { });

            "When constructing an argument using the type"
                .When(() => argument = new Argument(type));

            "Then the argument should be a generated default"
                .Then(() => argument.IsGeneratedDefault.Should().BeTrue());

            "And the argument value should be {1}"
                .And(() => argument.Value.Should().Be(expectedValue));
        }
    }
}
