// <copyright file="SpecificationAttributeFacts.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Legacy
{
    using System.Linq;
    using FakeItEasy;
    using Xbehave.Internal;
    using Xunit;
    using Xunit.Sdk;

    public static class SpecificationAttributeFacts
    {
        [Fact]
        public static void ReturnsNoCommandsForAMethodWithNoSteps()
        {
            SpecificationAttribute attribute = new SpecificationAttribute();

            var mock = A.Fake<IMethodInfo>();

            var commands = attribute.CreateTestCommands(mock);

            Assert.Empty(commands);
        }
    }
}