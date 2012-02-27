// <copyright file="ExpressionExtensionsSpecs.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace SubSpecGwt.Test.Naming
{
    using System;
    using System.Linq.Expressions;

    using FakeItEasy;

    using FluentAssertions;

    using SubSpec;
    using SubSpec.Naming;

    public static class ExpressionExtensionsSpecs
    {
        [Specification]
        public static void Spec1()
        {
            Specify(
                ((Expression<Action>)(() => 123.Should().Be(123))).Body,
                "123 should be 123");
        }

        [Specification]
        public static void Spec2()
        {
            var integer = default(int);
            Specify(
                ((Expression<Action>)(() => integer.Should().Be(123))).Body,
                "the integer should be 123");
        }

        [Specification]
        public static void Spec3()
        {
            var action = default(Action);
            Specify(
                ((Expression<Action>)(() => action.ShouldThrow<InvalidOperationException>())).Body,
                "the action should throw invalid operation exception");
        }

        [Specification]
        public static void Spec4()
        {
            var action = default(Action);
            Specify(
                ((Expression<Action>)(() => action.ShouldWibble4<Exception, ArgumentException>())).Body,
                "the action should wibble4 exception and argument exception");
        }

        public static void ShouldWibble4<T1, T2>(this Action action)
        {
        }

        [Specification]
        public static void Spec5()
        {
            var action = default(Action);
            Specify(
                ((Expression<Action>)(() => action.ShouldWibble5<Exception, ArgumentException, ArgumentNullException>())).Body,
                "the action should wibble5 exception, argument exception and argument null exception");
        }

        public static void ShouldWibble5<T1, T2, T3>(this Action action)
        {
        }

        [Specification]
        public static void Spec6()
        {
            var action = default(Action);
            Specify(
                ((Expression<Action>)(() => action.ShouldWibble6(123, 456L))).Body,
                "the action should wibble6 123 and 456L");
        }

        public static void ShouldWibble6(this Action action, int foo, long bar)
        {
        }

        [Specification]
        public static void Spec7()
        {
            var action = default(Action);
            Specify(
                ((Expression<Action>)(() => action.ShouldWibble7(123, 456L))).Body,
                "the action should wibble7 123 and 456L");
        }

        public static void ShouldWibble7<T1, T2>(this Action action, T1 foo, T2 bar)
        {
        }

        [Specification]
        public static void Spec8()
        {
            var action = default(Action);
            Specify(
                ((Expression<Action>)(() => action.ShouldWibble8<int, long>(123, 456L))).Body,
                "the action should wibble8 int32 and int64 with 123 and 456L");
        }

        public static void ShouldWibble8<T1, T2>(this Action action, int foo, long bar)
        {
        }

        [Specification]
        public static void Spec9()
        {
            var action = default(Action);
            Specify(
                ((Expression<Action>)(() => action.ShouldThrow<ArgumentException>().And.ParamName.Should().Be("foo"))).Body,
                "the action should throw argument exception and param name should be \"foo\"");
        }

        [Specification]
        public static void Spec10()
        {
            var foo = new object();
            Specify(
                ((Expression<Action>)(() => A.CallTo(() => foo.ToString()).MustHaveHappened(Repeated.Exactly.Once))).Body,
                "a call to (the foo to string) must have happened exactly once");
        }

        [Specification]
        public static void Spec11()
        {
            var foo = new object();
            Specify(
                ((Expression<Action>)(() => foo.Should().Be(null).And.Be((object)-1))).Body,
                "the foo should be null and be -1 converted to object");
        }

        [Specification]
        public static void Spec12()
        {
            var foo = new object();
            Specify(
                ((Expression<Action>)(() => foo.Should().Be(null).And.Be(-1 as object))).Body,
                "the foo should be null and be -1 as object");
        }

        [Specification]
        public static void Spec13()
        {
            var foo = new object();
            Specify(
                ((Expression<Action>)(() => foo.GetType().Should().Be(typeof(int)))).Body,
                "the foo get type should be int32");
        }

        [Specification]
        public static void Spec14()
        {
            var foo = 1;
            Specify(
                ((Expression<Action>)(() => A.CallTo(() => foo.ToString(A<string>.Ignored)).MustHaveHappened(Repeated.Exactly.Once))).Body,
                "a call to (the foo to string with some string) must have happened exactly once");
        }

        [Specification]
        public static void Spec15()
        {
            var foo = 1;
            Specify(
                ((Expression<Action>)(() => A.CallTo(() => foo.ToString(A<string>._)).MustHaveHappened(Repeated.Exactly.Once))).Body,
                "a call to (the foo to string with some string) must have happened exactly once");
        }

        [Specification]
        public static void Spec16()
        {
            var foo = 1;
            Specify(
                ((Expression<Action>)
                    (() => A.CallTo(() => foo.ToString(A<string>.That.Matches(value => value == "a"))).MustHaveHappened(Repeated.Exactly.Once))).Body,
                "a call to (the foo to string with some string that matches (value equals \"a\")) must have happened exactly once");
        }

        public static void Specify(Expression expression, string expectedName)
        {
            Specify(expression, expectedName, " ");
        }

        public static void Specify(Expression expression, string expectedName, string delimiter)
        {
            var name = default(string);

            "Given an expression and an expected name"
                .Given(() => { });

            "when creating"
                .When(() => name = expression.ToSpecName(delimiter));

            "the name should be the expected name"
                .Then(() => name.Should().Be(expectedName));
        }
    }
}
