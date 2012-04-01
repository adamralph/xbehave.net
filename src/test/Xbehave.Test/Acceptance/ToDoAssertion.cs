using System;
using Xbehave;
using Xunit;
using System.Xml;
using TestUtility;

public class TodoAssertion : SubSpecAcceptanceTest
{
	[Fact]
	public void OneSkippedTestPerTodo()
	{
		string code =
			@"
                using System;
                using Xunit;
				using Xbehave;

                public class MockTestClass
                {
						[Specification]
						public void MultipleAssertionsGenerateMultipleTests()
						{
							""A"".Context(() => { });

							""1"".Todo(() => { Assert.False(true); });
							""2"".Todo(() => { Assert.False(true); });
						}
                }
            ";

		XmlNode assemblyNode = ExecuteSpecification(code);

		var first = ResultXmlUtility.GetResult(assemblyNode, 0);
		var second = ResultXmlUtility.GetResult(assemblyNode, 1);

		ResultXmlUtility.AssertAttribute(first, "result", "Skip");
		ResultXmlUtility.AssertAttribute(second, "result", "Skip");
	}
}
