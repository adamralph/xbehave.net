// <copyright file="ExpressionExtensionsStory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Unit.Sdk.ExpressionNaming
{
    using System;
    using System.Linq.Expressions;

    using FakeItEasy;

    using FluentAssertions;

    using Xbehave;
    using Xbehave.Sdk.ExpressionNaming;

    public static class ExpressionExtensionsStory
    {
        [Scenario]
        public static void Expression1()
        {
            Scenario(
                ((Expression<Action>)(() => 123.Should().Be(123))).Body,
                "123 should be 123");
        }

        [Scenario]
        public static void Expression2()
        {
            var integer = default(int);
            Scenario(
                ((Expression<Action>)(() => integer.Should().Be(123))).Body,
                "the integer should be 123");
        }

        [Scenario]
        public static void Expression3()
        {
            var action = default(Action);
            Scenario(
                ((Expression<Action>)(() => action.ShouldThrow<InvalidOperationException>())).Body,
                "the action should throw invalid operation exception");
        }

        [Scenario]
        public static void Expression4()
        {
            var action = default(Action);
            Scenario(
                ((Expression<Action>)(() => action.ShouldWibble4<Exception, ArgumentException>())).Body,
                "the action should wibble4 exception and argument exception");
        }

        public static void ShouldWibble4<T1, T2>(this Action action)
        {
        }

        [Scenario]
        public static void Expression5()
        {
            var action = default(Action);
            Scenario(
                ((Expression<Action>)(() => action.ShouldWibble5<Exception, ArgumentException, ArgumentNullException>())).Body,
                "the action should wibble5 exception, argument exception and argument null exception");
        }

        public static void ShouldWibble5<T1, T2, T3>(this Action action)
        {
        }

        [Scenario]
        public static void Expression6()
        {
            var action = default(Action);
            Scenario(
                ((Expression<Action>)(() => action.ShouldWibble6(123, 456L))).Body,
                "the action should wibble6 123 and 456L");
        }

        public static void ShouldWibble6(this Action action, int foo, long bar)
        {
        }

        [Scenario]
        public static void Expression7()
        {
            var action = default(Action);
            Scenario(
                ((Expression<Action>)(() => action.ShouldWibble7(123, 456L))).Body,
                "the action should wibble7 123 and 456L");
        }

        public static void ShouldWibble7<T1, T2>(this Action action, T1 foo, T2 bar)
        {
        }

        [Scenario]
        public static void Expression8()
        {
            var action = default(Action);
            Scenario(
                ((Expression<Action>)(() => action.ShouldWibble8<int, long>(123, 456L))).Body,
                "the action should wibble8 int32 and int64 with 123 and 456L");
        }

        public static void ShouldWibble8<T1, T2>(this Action action, int foo, long bar)
        {
        }

        [Scenario]
        public static void Expression9()
        {
            var action = default(Action);
            Scenario(
                ((Expression<Action>)(() => action.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("foo"))).Body,
                "the action should throw argument exception and param name should be \"foo\"");
        }

        [Scenario]
        public static void Expression10()
        {
            var foo = new object();
            Scenario(
                ((Expression<Action>)(() => A.CallTo(() => foo.ToString()).MustHaveHappened(Repeated.Exactly.Once))).Body,
                "a call to (the foo to string) must have happened exactly once");
        }

        [Scenario]
        public static void Expression11()
        {
            var foo = new object();
            Scenario(
                ((Expression<Action>)(() => foo.Should().Be(null).And.Be((object)-1))).Body,
                "the foo should be null and be -1 converted to object");
        }

        [Scenario]
        public static void Expression12()
        {
            var foo = new object();
            Scenario(
                ((Expression<Action>)(() => foo.Should().Be(null).And.Be(-1 as object))).Body,
                "the foo should be null and be -1 as object");
        }

        [Scenario]
        public static void Expression13()
        {
            var foo = new object();
            Scenario(
                ((Expression<Action>)(() => foo.GetType().Should().Be(typeof(int)))).Body,
                "the foo get type should be int32");
        }

        [Scenario(Skip = "WIP")]
        public static void Expression14()
        {
            var foo = 1;
            Scenario(
                ((Expression<Action>)(() => A.CallTo(() => foo.ToString(A<string>.Ignored)).MustHaveHappened(Repeated.Exactly.Once))).Body,
                "a call to (the foo to string with some string) must have happened exactly once");
        }

        [Scenario(Skip = "WIP")]
        public static void Expression15()
        {
            var foo = 1;
            Scenario(
                ((Expression<Action>)(() => A.CallTo(() => foo.ToString(A<string>._)).MustHaveHappened(Repeated.Exactly.Once))).Body,
                "a call to (the foo to string with some string) must have happened exactly once");
        }

        [Scenario(Skip = "WIP")]
        public static void Expression16()
        {
            var foo = 1;
            Scenario(
                ((Expression<Action>)
                    (() => A.CallTo(() => foo.ToString(A<string>.That.Matches(value => value == "a"))).MustHaveHappened(Repeated.Exactly.Once))).Body,
                "a call to (the foo to string with some string that matches (value equals \"a\")) must have happened exactly once");
        }

        public static void Scenario(Expression expression, string expectedStepName)
        {
            Scenario(expression, expectedStepName, " ");
        }

        public static void Scenario(Expression expression, string expectedStepName, string delimiter)
        {
            var result = default(string);

            "Given an expression and an expected step name"
                .Given(() => { });

            "when converting the expression to a step name"
                .When(() => result = expression.ToSentence(delimiter))
                .Then(() => result.Should().Be(expectedStepName));
        }
    }
}
