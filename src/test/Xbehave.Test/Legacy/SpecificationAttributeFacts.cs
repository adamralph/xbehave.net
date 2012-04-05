// <copyright file="SpecificationAttributeFacts.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Legacy
{
    using System.Linq;
    using FakeItEasy;
    using Xbehave.Legacy;
    using Xunit;
    using Xunit.Sdk;

    public static class SpecificationAttributeFacts
    {
        [Fact]
        public static void ReportsExceptionWhenFailingToEnumerateTestCommands()
        {
            SpecificationAttribute attribute = new SpecificationAttribute();

            var mock = A.Fake<IMethodInfo>();

            var commands = attribute.CreateTestCommands(mock);

            Assert.IsAssignableFrom<ExceptionTestCommand>(commands.Single());
        }
    }
}