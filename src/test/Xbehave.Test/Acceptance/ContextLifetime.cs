using System;
using TestUtility;
using Xunit;
using System.Xml;
using Xbehave;

public class ContextLifetime : SubSpecAcceptanceTest
{
	private string template = @"
                        using System;
        				using Xbehave;
        
        				public class DisposableFixture : IDisposable
        				{
        					private bool _disposed = false;
                            public static int Instances = 0;

        					public DisposableFixture()
        					{
                                Console.WriteLine(""Fixture setup"");
        						Instances++;
        					}
        
        					public void Dispose()
        					{
        						if (_disposed)
        							throw new InvalidOperationException(""Already disposed!"");
        
        						Console.WriteLine(""Fixture disposed"");
        						_disposed = true;
                                Instances--;
        					}
        
        					public void Verify()
        					{
                                if (Instances != 1)
                                    throw new Exception(""exactly one instance is expected"");

        						if (!_disposed)
        							Console.WriteLine(""Fixture alive"");
        					}
        				}";

	[Fact]
	public void FreshContextForEachAssertionAndDisposedAfterExecution()
	{
		string code = template + @"
                public class MockTestClass
                {
					[Specification]
					public void Asserts()
					{
						DisposableFixture ctxt = default(DisposableFixture);

						"""".ContextFixture(() => ctxt = new DisposableFixture());
		
						"""".Assert(() => ctxt.Verify());
						"""".Assert(() => ctxt.Verify());
					}
                }";

		XmlNode assemblyNode = ExecuteSpecification(code);

        var first = assemblyNode.AssertionResults().First();
        var second = assemblyNode.AssertionResults().Second();

		const string expectedOutput = "Fixture setup\r\nFixture alive\r\nFixture disposed";

        SubSpecResultUtility.VerifyPassed(first);
        SubSpecResultUtility.VerifyOutput(first, expectedOutput);

        SubSpecResultUtility.VerifyPassed(second);
        SubSpecResultUtility.VerifyOutput(second, expectedOutput);
	}

	[Fact]
    public void ContextSharedBetweenObservationsAndLifecycleManagedExternally()
    {
        string code = template + @"
                public class MockTestClass
                {
					[Specification]
					public void Observations()
					{
						DisposableFixture ctxt = default(DisposableFixture);

						"""".ContextFixture(() => ctxt = new DisposableFixture());
						"""".Do(() => { } );

						"""".Observation(() => ctxt.Verify());
						"""".Observation(() => ctxt.Verify());
					}
                }";

        XmlNode assemblyNode = ExecuteSpecification(code);

        VerifyFixtureSetup(assemblyNode.ObservationSetupResult());
        VerifyFixtureAlive(assemblyNode.ObservationResults().First());
        VerifyFixtureAlive(assemblyNode.ObservationResults().Second());
        VerifyFixtureDisposed(assemblyNode.ObservationTeardownResult());
    }

    private static void VerifyFixtureSetup(XmlNode testNode)
    {
        SubSpecResultUtility.VerifyPassed(testNode);
        SubSpecResultUtility.VerifyOutput(testNode, "Fixture setup");
    }
    private static void VerifyFixtureAlive(XmlNode testNode)
	{
        SubSpecResultUtility.VerifyPassed(testNode);
        SubSpecResultUtility.VerifyOutput(testNode, "Fixture alive");
	}
    private static void VerifyFixtureDisposed(XmlNode testNode)
    {
        SubSpecResultUtility.VerifyPassed(testNode);
        SubSpecResultUtility.VerifyOutput(testNode, "Fixture disposed");
    }
}



