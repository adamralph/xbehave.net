// <copyright file="ExceptionTestCommandFacts.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Internal
{
    using FakeItEasy;
    using FluentAssertions;
    using Xbehave.Internal;
    using Xunit;
    using Xunit.Sdk;

    public static class ExceptionTestCommandFacts
    {
        [Fact]
        public static void ShouldNotCreateInstance()
        {
            // arrange
            var target = new ExceptionTestCommand(A.Fake<IMethodInfo>(), () => { });

            // act
            var shouldCreateInstance = target.ShouldCreateInstance;

            // assert
            shouldCreateInstance.Should().BeFalse();
        }
    }
}
