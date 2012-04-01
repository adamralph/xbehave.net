using System;
using TestUtility;
using Xunit;
using System.Xml;
using Xbehave;
using System.Linq;

public class OutputCapturing : SubSpecAcceptanceTest
{
    [Fact]
    public void OutputInContextWithObservationIsCaptured()
    {
        string code =
            @"
                using System;
                using Xunit;
				using Xbehave;

                public class MockTestClass
                {
                    [Specification]
                    public void TestOutputInDoActionIsCaptured()
                    {
                        ""Context"".Context( () => { Console.Error.WriteLine(""err""); Console.WriteLine( ""out"" ); } );
                        ""do"".Do( () => { } );
                        ""Observe"".Observation( () => { } );
                    }
                }
            ";

        XmlNode assemblyNode = ExecuteSpecification(code);

        VerifyStdAndErrOutputCaptured(assemblyNode.ObservationSetupResult());
    }

    [Fact]
    public void OutputInContextWithAssertIsCaptured()
    {
        string code =
            @"
                using System;
                using Xunit;
				using Xbehave;

                public class MockTestClass
                {
                    [Specification]
                    public void TestOutputInDoActionIsCaptured()
                    {
                        ""Context"".Context( () => { Console.Error.WriteLine(""err""); Console.WriteLine( ""out"" ); } );
                        ""do"".Do( () => { } );
                        ""Assert"".Assert( () => { } );
                    }
                }
            ";

        
        XmlNode assemblyNode = ExecuteSpecification(code);

        VerifyStdAndErrOutputCaptured(assemblyNode.AssertionResults().First());

    }
    [Fact]
    public void OutputInDoWithObservationIsCaptured()
    {
        string code =
            @"
                using System;
                using Xunit;
				using Xbehave;

                public class MockTestClass
                {
                    [Specification]
                    public void TestOutputInDoActionIsCaptured()
                    {
                        ""Context"".Context( () => { } );
                        ""do"".Do( () => { Console.Error.WriteLine(""err""); Console.WriteLine( ""out"" );} );
                        ""Observe"".Observation( () => { } );
                    }
                }
            ";

        XmlNode assemblyNode = ExecuteSpecification(code);

        VerifyStdAndErrOutputCaptured(assemblyNode.ObservationSetupResult());
    }

    [Fact]
    public void OutputInDoWithAssertIsCaptured()
    {
        string code =
            @"
                using System;
                using Xunit;
				using Xbehave;

                public class MockTestClass
                {
                    [Specification]
                    public void TestOutputInDoActionIsCaptured()
                    {
                        ""Context"".Context( () => { } );
                        ""do"".Do( () => { Console.Error.WriteLine(""err""); Console.WriteLine( ""out"" );} );
                        ""Assert"".Assert( () => { } );
                    }
                }
            ";

        XmlNode assemblyNode = ExecuteSpecification(code);

        VerifyStdAndErrOutputCaptured(assemblyNode.AssertionResults().First());
    }

    [Fact]
    public void OutputInObserveCaptured()
    {
        string code =
            @"
                using System;
                using Xunit;
				using Xbehave;

                public class MockTestClass
                {
                    [Specification]
                    public void TestOutputInDoActionIsCaptured()
                    {
                        ""Context"".Context( () => { } );
                        ""do"".Do( () => { } );
                        ""Observe"".Observation( () => { Console.Error.WriteLine(""err""); Console.WriteLine( ""out"" );} );
                    }
                }
            ";

        XmlNode assemblyNode = ExecuteSpecification(code);

        VerifyStdAndErrOutputCaptured(assemblyNode.ObservationResults().First());
    }

    [Fact]
    public void OutputInAssertIsCaptured()
    {
        string code =
            @"
                using System;
                using Xunit;
				using Xbehave;

                public class MockTestClass
                {
                    [Specification]
                    public void TestOutputInDoActionIsCaptured()
                    {
                        ""Context"".Context( () => { } );
                        ""do"".Do( () => { } );
                        ""Assert"".Assert( () => { Console.Error.WriteLine(""err""); Console.WriteLine( ""out"" );} );
                    }
                }
            ";

        XmlNode assemblyNode = ExecuteSpecification(code);

        VerifyStdAndErrOutputCaptured(assemblyNode.AssertionResults().First());
    }

    private static void VerifyStdAndErrOutputCaptured( XmlNode first )
    {
        SubSpecResultUtility.VerifyPassed(first);

        XmlNode outputNode = first.SelectSingleNode( "output" );

        Assert.Equal( "err\r\nout\r\n", outputNode.InnerXml );
    }
}

