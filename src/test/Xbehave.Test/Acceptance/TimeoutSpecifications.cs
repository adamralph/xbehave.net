using System;
using TestUtility;
using Xunit;
using System.Xml;
using Xbehave;

public class TimeoutSpecifications : SubSpecAcceptanceTest
{
    const string USINGS = @"
                using System;
                using System.Threading;
                using Xunit;
				using Xbehave;";

    [Fact]
    public void TimeoutInContextWithAssertExecutorMakesTestFail()
    {
        string code = USINGS + @"
                public class MockTestClass
                {
					[Specification]
					public void Spec()
					{
						"""".Context(() => {Thread.Sleep(1000);}).WithTimeout(10);
						"""".Assert(() => {});
					}
                }";

        XmlNode assemblyNode = ExecuteSpecification(code);

        VerifyTimeoutOccurred( assemblyNode.AssertionResults().First() );
    }

    [Fact]
    public void TimeoutInContextWithObservationExecutorMakesTestFail()
    {
        string code = USINGS + @"
                public class MockTestClass
                {
					[Specification]
					public void Spec()
					{
						"""".Context(() => {Thread.Sleep(1000);}).WithTimeout(10);
						"""".Observation(() => {});
					}
                }";

        XmlNode assemblyNode = ExecuteSpecification(code);

        VerifyTimeoutOccurred( assemblyNode.ObservationSetupResult() );
    }

    [Fact]
    public void TimeoutInDoWithAssertExecutorMakesTestFail()
    {
        string code = USINGS + @"
                public class MockTestClass
                {
					[Specification]
					public void Spec()
					{
						"""".Context(() => {} );
		                """".Do( () => {Thread.Sleep(1000);} ).WithTimeout(10);
						"""".Assert(() => {});
					}
                }";

        XmlNode assemblyNode = ExecuteSpecification(code);

        VerifyTimeoutOccurred( assemblyNode.AssertionResults().First() );
    }

    [Fact]
    public void TimeoutInDoWithObservationExecutorMakesTestFail()
    {
        string code = USINGS + @"
                public class MockTestClass
                {
					[Specification]
					public void Spec()
					{
						"""".Context(() => {} );
		                """".Do( () => {Thread.Sleep(1000);} ).WithTimeout(10);
						"""".Observation(() => {});
					}
                }";

        XmlNode assemblyNode = ExecuteSpecification(code);

        VerifyTimeoutOccurred( assemblyNode.ObservationSetupResult() );
    }

    [Fact]
    public void TimeoutInAssertMakesTestFail()
    {
        string code = USINGS + @"
                public class MockTestClass
                {
					[Specification]
					public void Spec()
					{
						"""".Context(() => {} );
		                """".Do( () => {} );
						"""".Assert(() => {Thread.Sleep(1000);}).WithTimeout(10);
					}
                }";

        XmlNode assemblyNode = ExecuteSpecification(code);

        VerifyTimeoutOccurred( assemblyNode.AssertionResults().First() );
    }

    [Fact]
    public void TimeoutInObservationMakesTestFail()
    {
        string code = USINGS + @"
                public class MockTestClass
                {
					[Specification]
					public void Spec()
					{
						"""".Context(() => {} );
		                """".Do( () => {} );
						"""".Observation(() => {Thread.Sleep(1000);}).WithTimeout(10);
					}
                }";

        XmlNode assemblyNode = ExecuteSpecification(code);

        VerifyTimeoutOccurred( assemblyNode.ObservationResults().First() );
    }

    private static void VerifyTimeoutOccurred( XmlNode first )
    {
        ResultXmlUtility.AssertAttribute( first, "result", "Fail" );

        XmlNode firstFailureNode = first.SelectSingleNode( "failure" );
        Assert.Equal( "Xunit.Sdk.TimeoutException", firstFailureNode.Attributes.GetNamedItem( "exception-type" ).InnerXml );
        Assert.Equal( "Test execution time exceeded: 10ms", firstFailureNode.SelectSingleNode( "message" ).InnerXml );
    }
}