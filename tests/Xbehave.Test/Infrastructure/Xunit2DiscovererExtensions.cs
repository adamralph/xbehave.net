namespace Xbehave.Test.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Abstractions;

    public static class Xunit2DiscovererExtensions
    {
        public static Queue<ITestCase> Find(this Xunit2Discoverer discoverer, Type type, TestAssemblyConfiguration testAssemblyConfiguration)
        {
            using (var sink = new SpyMessageSink<IDiscoveryCompleteMessage>())
            {
                discoverer.Find(type.FullName, false, sink, TestFrameworkOptions.ForDiscovery(testAssemblyConfiguration));
                sink.Finished.Wait();

                return new Queue<ITestCase>(
                    sink.Messages.OfType<ITestCaseDiscoveryMessage>()
                        .Select(message => message.TestCase));
            }
        }

        public static Queue<ITestCase> Find(this Xunit2Discoverer discoverer, string collectionName, TestAssemblyConfiguration testAssemblyConfiguration)
        {
            using (var sink = new SpyMessageSink<IDiscoveryCompleteMessage>())
            {
                discoverer.Find(false, sink, TestFrameworkOptions.ForDiscovery(testAssemblyConfiguration));
                sink.Finished.Wait();

                return new Queue<ITestCase>(
                    sink.Messages.OfType<ITestCaseDiscoveryMessage>()
                        .Select(message => message.TestCase)
                        .Where(testCase => testCase.TestMethod.TestClass.TestCollection.DisplayName == collectionName));
            }
        }

        public static Queue<ITestCase> Find(this Xunit2Discoverer discoverer, Type type, string traitName, string traitValue, TestAssemblyConfiguration testAssemblyConfiguration)
        {
            using (var sink = new SpyMessageSink<IDiscoveryCompleteMessage>())
            {
                discoverer.Find(type.FullName, false, sink, TestFrameworkOptions.ForDiscovery(testAssemblyConfiguration));
                sink.Finished.Wait();

                return new Queue<ITestCase>(
                    sink.Messages.OfType<ITestCaseDiscoveryMessage>()
                        .Select(message => message.TestCase)
                        .Where(testCase => testCase.Traits.TryGetValue(traitName, out var values) && values.Contains(traitValue)));
            }
        }
    }
}
