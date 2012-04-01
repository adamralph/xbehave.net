using System;
using TestUtility;
using Xunit;
using System.Xml;
using Xbehave;
using System.Linq;

public class ContextSetupTeardownFailure : SubSpecAcceptanceTest
{
	[Fact]
	public void ErrorInContextSetupMarksAllAssertionsFailed()
	{
		string code =
			@"
                using System;
                using Xunit;
				using Xbehave;

                public class MockTestClass
                {
					[Specification]
					public void ErrorInContextSetup()
					{
						""Context"".ContextFixture(() => { throw new Exception(""Error setting up context""); } );
						
						""A"".Assert(() => { Assert.True(true); });
						""B"".Assert(() => { Assert.True(true); });
					}
                }
            ";

		XmlNode assemblyNode = ExecuteSpecification(code);

        SubSpecResultUtility.VerifyFailedWith( assemblyNode.AssertionResults().First(), "System.Exception : Error setting up context" );
        SubSpecResultUtility.VerifyFailedWith( assemblyNode.AssertionResults().Second(), "System.Exception : Error setting up context" );
	}

	[Fact]
	public void ErrorInContextTeardownMarksAllAssertionsFailed()
	{
		string code =
            @"
                using System;
                using Xunit;
				using Xbehave;

                public class MockTestClass
                {
					[Specification]
					public void ErrorInContextSetup()
					{
						""Context"".ContextFixture(() => new DisposableAction( () => { throw new Exception(""Error tearing down context""); } ));
						
						""A"".Assert(() => { Console.WriteLine(""A""); });
						""B"".Assert(() => { Console.WriteLine(""B""); });
					}
                }
            ";

		XmlNode assemblyNode = ExecuteSpecification(code);

        string expectedFailure = "System.Exception : Error tearing down context";

        SubSpecResultUtility.VerifyFailedWith( assemblyNode.AssertionResults().First(), expectedFailure );
        SubSpecResultUtility.VerifyFailedWith( assemblyNode.AssertionResults().Second(), expectedFailure );
	}
	
	[Fact]
	public void ErrorInObservationContextSetupMarksSpecificationFailed()
	{
		string code =
			@"
                using System;
                using Xunit;
				using Xbehave;

                public class MockTestClass
                {
					[Specification]
					public void ErrorInContextSetup()
					{
						""Context"".ContextFixture(() => { throw new Exception(""Error setting up context""); } );
						
						""A"".Observation(() => { Assert.True(true); });
						""B"".Observation(() => { Assert.True(true); });
					}
                }
            ";


		XmlNode assemblyNode = ExecuteSpecification(code);

        SubSpecResultUtility.VerifyFailedWith( assemblyNode.ObservationSetupResult(), "System.Exception : Error setting up context" );
        SubSpecResultUtility.VerifyFailedWith( assemblyNode.ObservationResults().First(), "SubSpec.Core+ContextSetupFailedException : Setting up Context failed" );
        SubSpecResultUtility.VerifyFailedWith( assemblyNode.ObservationResults().Second(), "SubSpec.Core+ContextSetupFailedException : Setting up Context failed" );
        SubSpecResultUtility.VerifyFailedWith( assemblyNode.ObservationTeardownResult(), "SubSpec.Core+ContextSetupFailedException : Setting up Context failed, but Fixtures were disposed." );
    }

	[Fact]
	public void ErrorInContextTeardownPassesObservationsButMarksTeardownFailed()
	{
		string code =
            @"
                using System;
                using Xunit;
				using Xbehave;

                public class MockTestClass
                {
					[Specification]
					public void ErrorInContextSetup()
					{
						""Context"".ContextFixture(() => new DisposableAction( () => { throw new Exception(""Error tearing down context""); } ));
						
						""A"".Observation(() => { Console.WriteLine(""A""); });
						""B"".Observation(() => { Console.WriteLine(""B""); });
					}
                }
            ";

		XmlNode assemblyNode = ExecuteSpecification(code);

        SubSpecResultUtility.VerifyPassed( assemblyNode.ObservationSetupResult() );
        SubSpecResultUtility.VerifyPassed( assemblyNode.ObservationResults().First() );
        SubSpecResultUtility.VerifyPassed( assemblyNode.ObservationResults().Second());
        SubSpecResultUtility.VerifyFailedWith( assemblyNode.ObservationTeardownResult(), "System.Exception : Error tearing down context" );
	}
}