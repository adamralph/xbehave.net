using System;
using System.Linq;
using Xunit;
using Xbehave;
using Xunit.Sdk;
using FakeItEasy;

public class SpecificationAttributeFacts
{
	[Fact]
	public void ReportsExceptionWhenFailingToEnumerateTestCommands()
	{
		SpecificationAttribute attribute = new SpecificationAttribute();

        var method = A.Fake<IMethodInfo>();
        
        var commands = attribute.CreateTestCommands(method);

		// Assert.IsAssignableFrom<SubSpec.Core.ExceptionTestCommand>(commands.Single().);
	}
}
