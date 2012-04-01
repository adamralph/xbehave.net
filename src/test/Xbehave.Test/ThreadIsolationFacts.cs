using System;
using Xunit;
using System.Threading;
using Xbehave;

public class ThreadIsolationFacts
{
    [Fact]
    public void CanSetUpSpecificationConcurrently()
    {
        VerifyConcurrentExecution( SetUpSpecification );
    }

    private void VerifyConcurrentExecution( ThreadStart action )
    {
        Assert.DoesNotThrow( () =>
        {
            // We could use the Task API here but we want to be explicit about getting two concurrent, physical threads
            Thread a = new Thread( action );
            Thread b = new Thread( action );

            a.Start();
            b.Start();

            a.Join();
            b.Join();
        } );
    }

    private void SetUpSpecification()
    {
        "".Given( () => { } );
        "".When( () => { } );
        "".Assert( () => { } );
    }

    [Fact]
    public void DoEnsuresThreadStatic()
    {
        VerifyConcurrentExecution( () => "".When( () => { } ) );
    }

    [Fact]
    public void AssertEnsuresThreadStatic()
    {
        VerifyConcurrentExecution( () => "".Assert( () => { } ) );
    }

    [Fact]
    public void ObservationEnsuresThreadStatic()
    {
        VerifyConcurrentExecution( () => "".Observation( () => { } ) );
    }

    [Fact]
    public void TodoEnsuresThreadStatic()
    {
        VerifyConcurrentExecution( () => "".Todo( () => { } ) );
    }

    [Fact]
    public void CanEnumerateTestCommandsOfEmptySpecificationConcurrently()
    {
        VerifyConcurrentExecution( () => Xbehave.ScenarioContext.SafelyEnumerateTestCommands( null, _ => { } ) );
    }
}
