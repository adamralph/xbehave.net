// <copyright file="SpecificationAttributeFacts.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test
{
    using FakeItEasy;
    using Xbehave;
    using Xunit;
    using Xunit.Sdk;

    public class SpecificationAttributeFacts
    {
        [Fact]
        public void ReportsExceptionWhenFailingToEnumerateTestCommands()
        {
            var attribute = new ScenarioAttribute();

            var method = A.Fake<IMethodInfo>();

            var commands = attribute.CreateTestCommands(method);

            // Assert.IsAssignableFrom<SubSpec.Core.ExceptionTestCommand>(commands.Single().);
        }
    }
}