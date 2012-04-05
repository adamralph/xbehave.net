// <copyright file="ThesisAttributeFacts.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Legacy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FakeItEasy;
    using Xunit;
    using Xunit.Extensions;
    using Xunit.Sdk;

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
            Assert.Equal(6, commands.Count());
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
