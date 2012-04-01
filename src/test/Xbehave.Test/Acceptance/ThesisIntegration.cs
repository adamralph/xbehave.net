using System;
using Xbehave;
using Xunit;
using System.Xml;
using TestUtility;
using Xunit.Extensions;
using System.Linq;

public class ThesisIntegration : SubSpecAcceptanceTest
{
    [Fact]
    public void OneSpecificationPerThesisDataItem()
    {
        string code =
            @"
                using System;
                using Xunit;
				using Xunit.Extensions;
				using Xbehave;

                public class MockTestClass
                {
						[Thesis]
						[InlineData(1, ""A"")]
						[InlineData(2, ""B"")]
						public void OneSpecificationPerThesisDataItem(int i, string s)
						{
							i.ToString().Context(() => { });

							(s + ""= A?"").Assert(() => { Assert.Equal(""A"", s); });
							(s + ""= B?"").Assert(() => { Assert.Equal(""B"", s); });
						}
                }
            ";

        XmlNode assemblyNode = ExecuteThesis( code );

        // Parameter "A"
        SubSpecResultUtility.VerifyPassed( assemblyNode.AssertionResults().First() );
        SubSpecResultUtility.VerifyFailed( assemblyNode.AssertionResults().Second() );

        // Parameter "B"
        SubSpecResultUtility.VerifyFailed( assemblyNode.AssertionResults().ElementAt( 2 ) );
        SubSpecResultUtility.VerifyPassed( assemblyNode.AssertionResults().ElementAt( 3 ) );
    }

    [Fact]
    public void ThesisHandlesExceptionsInSpecificationSetup()
    {
        string code =
            @"
                using System;
                using Xunit;
				using Xunit.Extensions;
				using Xbehave;

                public class MockTestClass
                {
						[Thesis]
						[InlineData(""A"")]
						[InlineData(""B"")]
						public void Throws(string s)
						{
							throw new Exception(""Error in Specification"");
						}
                }
            ";

        XmlNode assemblyNode = ExecuteThesis( code );

        var first = ResultXmlUtility.GetResult( assemblyNode, 0 );
        var second = ResultXmlUtility.GetResult( assemblyNode, 1 );

        string expectedException = "System.InvalidOperationException : An exception was thrown while building tests from Specification MockTestClass.Throws:\r\nSystem.Exception: Error in Specification";
        SubSpecResultUtility.VerifyFailedWith( first, expectedException );
        SubSpecResultUtility.VerifyFailedWith( second, expectedException );
    }

    public void ThesisHandlesNoDataInTheory()
    {
        string code =
            @"
                using System;
                using Xunit;
				using Xunit.Extensions;
				using Xbehave;

                public class MockTestClass
                {
						[Thesis]
						public void Spec()
						{
							"""".Context(() => { });
							"""".Observation(() => { });
						}
                }
            ";

        XmlNode assemblyNode = ExecuteThesis( code );

        var first = ResultXmlUtility.GetResult( assemblyNode, 0 );

        SubSpecResultUtility.VerifyFailedWith( first, "System.InvalidOperationException : No data found for MockTestClass.Spec" );
    }
}
