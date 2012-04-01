using System;
using Xunit;
using System.Xml;
using TestUtility;
using Xbehave;

public class SpecificationDeclarations : SubSpecAcceptanceTest
{
    private const string Usings = @" 
                using System;
                using Xunit;
				using Xbehave;";
    [Fact]
    public void StaticSpecificationOnStaticClass()
    {
        string code =
            Usings + @"

                public static class MockTestClass
                {
					[Specification]
					public static void Spec()
					{
						"""".Context(() => {} );
						"""".Assert(() => {});
					}
                }
            ";

        XmlNode assemblyNode = ExecuteSpecification( code );

        SubSpecResultUtility.VerifyPassed( assemblyNode.AssertionResults().First() );
    }

    [Fact]
    public void StaticSpecificationOnClass()
    {
        string code =
            Usings + @"

                public class MockTestClass
                {
					[Specification]
					public static void Spec()
					{
						"""".Context(() => {} );
						"""".Assert(() => {});
					}
                }
            ";

        XmlNode assemblyNode = ExecuteSpecification( code );

        SubSpecResultUtility.VerifyPassed( assemblyNode.AssertionResults().First() );
    }

    [Fact]
    public void SpecificationOnClassWithoutDefaultConstructorThrows()
    {
        string code =
            Usings + @"

                public class MockTestClass
                {
                    private MockTestClass(){}

					[Specification]
					public void Spec()
					{
						"""".Context(() => {} );
						"""".Assert(() => {});
					}
                }
            ";

        XmlNode assemblyNode = ExecuteSpecification( code );

        string expected = "System.InvalidOperationException : An exception was thrown while building tests from Specification MockTestClass.Spec:\r\nSystem.InvalidOperationException: Specification class does not have a default constructor";

        SubSpecResultUtility.VerifyFailedWith( assemblyNode.AssertionResults().First(), expected );
    }
}
