// <copyright file="SpecificationExtensionsFacts.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Legacy
{
    using System;
    using FakeItEasy;
    using Xunit;

    public static class SpecificationExtensionsFacts
    {
        [Fact]
        public static void ThrowsInvalidOperationExceptionWhenDeclaringContextWithContextDelegate()
        {
            // arrange
            var message = "foo";
            var contextDelegate = new ContextDelegate(() => A.Fake<IDisposable>());

            // act
            var exception = Record.Exception(() => message.Context(contextDelegate));

            // assert
            Assert.IsType<InvalidOperationException>(exception);
        }
    }
}
