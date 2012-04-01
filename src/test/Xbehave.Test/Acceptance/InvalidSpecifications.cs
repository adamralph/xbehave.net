using System;
using Xbehave;
using Xunit;
using TestUtility;
using System.Xml;

public class InvalidSpecifications : SubSpecAcceptanceTest
{
	[Fact]
	public void SpecificationInvalidWithoutContextThrowsExpectedException()
	{
		string code =
			@"
                using System;
                using Xunit;
				using Xbehave;

                public class MockTestClass
                {
					[Specification]
					public void SpecificationInvalidWithoutContext()
					{
						""without context"".Do(() => { });
						""we expect our specification to be invalid and hence not executed"".Assert(() => { Assert.True(true); });
					}
                }
            ";

        XmlNode assemblyNode = ExecuteSpecification(code);

		XmlNode testNode = ResultXmlUtility.AssertResult(assemblyNode, "Fail", "MockTestClass.SpecificationInvalidWithoutContext");

		XmlNode failureNode = testNode.SelectSingleNode("failure");

		Assert.Equal("System.InvalidOperationException : Must have a Context in each specification", failureNode.SelectSingleNode("message").InnerXml);
	}

	[Fact]
	public void SpecificationInvalidWithoutAssertOrObservationThrowsExpectedException()
	{
		string code =
			@"
                using System;
                using Xunit;
				using Xbehave;

                public class MockTestClass
                {
					[Specification]
					public void SpecificationInvalidWithoutAssertOrObservation()
					{
						""Given a context"".Context(() => { });
						""and an action but no observations or assertions"".Do(() => { });
					}
                }
            ";

		XmlNode assemblyNode = ExecuteSpecification(code);

		XmlNode testNode = ResultXmlUtility.AssertResult(assemblyNode, "Fail", "MockTestClass.SpecificationInvalidWithoutAssertOrObservation");

		XmlNode failureNode = testNode.SelectSingleNode("failure");

		Assert.Equal("System.InvalidOperationException : Must have at least one Assert or Observation in each specification", failureNode.SelectSingleNode("message").InnerXml);
	}

	[Specification]
	public void SpecificationNotInvalidWithoutAction()
	{
		"Given a context".Context(() => { });
		// but no action
		"we expect our specification to be valid and this assertion to be executed.".Assert(() => { Assert.True(true); });
	}

	[Fact]
	public void SpecificationInvalidWithMoreThanOneContext()
	{
		string code =
			@"
                using System;
                using Xunit;
				using Xbehave;

                public class MockTestClass
                {
					[Specification]
					public void SpecificationInvalidWithMoreThanOneContext()
					{
						""A"".Context(() => { });
						""B"".Context(() => { });
					}
                }
            ";

		XmlNode assemblyNode = ExecuteSpecification(code);

		XmlNode testNode = ResultXmlUtility.AssertResult(assemblyNode, "Fail", "MockTestClass.SpecificationInvalidWithMoreThanOneContext");

		XmlNode failureNode = testNode.SelectSingleNode("failure");

		Assert.Equal("System.InvalidOperationException : Cannot have more than one Context statement in a specification", failureNode.SelectSingleNode("message").InnerXml);
	}

	[Fact]
	public void SpecificationInvalidWithMoreThanOneDo()
	{
		string code =
			@"
                using System;
                using Xunit;
				using Xbehave;

                public class MockTestClass
                {
					[Specification]
					public void SpecificationInvalidWithMoreThanOneDo()
					{
						""Given a context"".Context(() => { });
						""A"".Do(() => { });
						""B"".Do(() => { });
					}
                }
            ";

		XmlNode assemblyNode = ExecuteSpecification(code);

		XmlNode testNode = ResultXmlUtility.AssertResult(assemblyNode, "Fail", "MockTestClass.SpecificationInvalidWithMoreThanOneDo");

		XmlNode failureNode = testNode.SelectSingleNode("failure");

		Assert.Equal("System.InvalidOperationException : Cannot have more than one Do statement in a specification", failureNode.SelectSingleNode("message").InnerXml);
	}
}
