// <copyright file="ThreadIsolationFacts.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Unit.Legacy
{
    using System.Threading;
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
            VerifyConcurrentExecution(() => "foo".Then(() => { }).InIsolation());
        }

        [Fact]
        public static void ObservationEnsuresThreadStatic()
        {
            VerifyConcurrentExecution(() => "foo".Then(() => { }));
        }

        [Fact]
        public static void TodoEnsuresThreadStatic()
        {
            VerifyConcurrentExecution(() => "foo".Then(() => { }).Skip("for some reason"));
        }

        private static void VerifyConcurrentExecution(ThreadStart action)
        {
            Assert.DoesNotThrow(() =>
            {
                // We could use the Task API here but we want to be explicit about getting two concurrent, physical threads
                var a = new Thread(action);
                var b = new Thread(action);

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
            "foo".Then(() => { }).InIsolation();
        }
    }
}