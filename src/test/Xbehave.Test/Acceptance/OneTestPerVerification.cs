using System;
using Xbehave;
using Xunit;
using System.Xml;
using TestUtility;

public class OneTestPerVerification : SubSpecAcceptanceTest
{
	[Fact]
	public void OneTestPerAssert()
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

							""1"".Assert(() => { Assert.True(true); });
							""2"".Assert(() => { Assert.True(true); });
						}
                }
            ";

		XmlNode assemblyNode = ExecuteSpecification(code);

        SubSpecResultUtility.VerifyPassed( assemblyNode.AssertionResults().First() );
        SubSpecResultUtility.VerifyPassed( assemblyNode.AssertionResults().Second() );
	}

	[Fact]
	public void OneTestPerObservationPlusSetupAndTeardown()
	{
		string code =
			@"
                using System;
                using Xunit;
				using Xbehave;

                public class MockTestClass
                {
						[Specification]
						public void MultipleObservationsGenerateMultipleTests()
						{
							""A"".Context(() => { });

							""1"".Observation(() => { Assert.True(true); });
							""2"".Observation(() => { Assert.True(true); });
						}
                }
            ";

		XmlNode assemblyNode = ExecuteSpecification(code);

        SubSpecResultUtility.VerifyPassed( assemblyNode.ObservationSetupResult() );
        SubSpecResultUtility.VerifyPassed( assemblyNode.ObservationResults().First() );
        SubSpecResultUtility.VerifyPassed( assemblyNode.ObservationResults().Second() );
        SubSpecResultUtility.VerifyPassed( assemblyNode.ObservationTeardownResult() );
	}

}
