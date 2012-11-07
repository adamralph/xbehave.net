// <copyright file="ThesisAttributeFacts.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Unit.Deprecated
{
    using System.Linq;
    using FluentAssertions;
    using Xunit;
    using Xunit.Extensions;

    public static class ThesisAttributeFacts
    {
        [Fact]
        public static void EnumeratesTestCommands()
        {
            // arrange
            var target = new ThesisAttribute();
            var method = Xunit.Sdk.Reflector.Wrap(typeof(ThesisAttributeFacts).GetMethod("Foo"));

            // act
            var commands = target.CreateTestCommands(method);

            // assert
            commands.Count().Should().Be(18);
        }

        [Thesis]
        [InlineData("a")]
        [InlineData("b")]
        [InlineData("c")]
        public static void Foo(string bar)
        {
            "given".Context(() => { });
            "when".Do(() => { });
            "then".Assert(() => { });
            "then".Assert(() => { });
        }
    }
}
