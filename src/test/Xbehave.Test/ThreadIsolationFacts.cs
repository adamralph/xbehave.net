// <copyright file="ThreadIsolationFacts.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test
{
    using System.Threading;
    using Xbehave;
    using Xunit;

    public static class ThreadIsolationFacts
    {
        [Fact]
        public static void CanSetUpSpecificationConcurrently()
        {
            VerifyConcurrentExecution(SetUpSpecification);
        }

        [Fact]
        public static void DoEnsuresThreadStatic()
        {
            VerifyConcurrentExecution(() => "foo".When(() => { }));
        }

        [Fact]
        public static void AssertEnsuresThreadStatic()
        {
            VerifyConcurrentExecution(() => "foo".ThenInIsolation(() => { }));
        }

        [Fact]
        public static void ObservationEnsuresThreadStatic()
        {
            VerifyConcurrentExecution(() => "foo".Then(() => { }));
        }

        [Fact]
        public static void TodoEnsuresThreadStatic()
        {
            VerifyConcurrentExecution(() => "foo".ThenSkip(() => { }));
        }

        [Fact]
        public static void CanEnumerateTestCommandsOfEmptySpecificationConcurrently()
        {
            VerifyConcurrentExecution(() => Xbehave.ScenarioContext.SafelyEnumerateTestCommands(null, _ => { }));
        }

        private static void VerifyConcurrentExecution(ThreadStart action)
        {
            Assert.DoesNotThrow(() =>
            {
                // We could use the Task API here but we want to be explicit about getting two concurrent, physical threads
                Thread a = new Thread(action);
                Thread b = new Thread(action);

                a.Start();
                b.Start();

                a.Join();
                b.Join();
            });
        }

        private static void SetUpSpecification()
        {
            "foo".Given(() => { });
            "foo".When(() => { });
            "foo".ThenInIsolation(() => { });
        }
    }
}