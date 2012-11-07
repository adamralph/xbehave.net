// <copyright file="SpecificationExtensionsFacts.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Unit.Deprecated
{
    using System;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public static class SpecificationExtensionsFacts
    {
        [Fact]
        public static void ThrowsInvalidOperationExceptionWhenDeclaringContextWithContextDelegate()
        {
            // arrange
            var message = "foo";
            var contextDelegate = new ContextDelegate(A.Fake<IDisposable>);

            // act
            var exception = Record.Exception(() => message.Context(contextDelegate));

            // assert
            exception.Should().BeOfType<InvalidOperationException>();
        }
    }
}
