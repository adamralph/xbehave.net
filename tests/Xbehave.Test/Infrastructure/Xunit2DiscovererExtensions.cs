namespace Xbehave.Test.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Abstractions;

    public static class Xunit2DiscovererExtensions
    {
        public static IEnumerable<ITestCase> Find(this Xunit2Discoverer discoverer, string collectionName)
        {
            using (var sink = new SpyMessageSink<IDiscoveryCompleteMessage>())
            {
                discoverer.Find(false, sink, TestFrameworkOptions.ForDiscovery());
                sink.Finished.WaitOne();
                return sink.Messages.OfType<ITestCaseDiscoveryMessage>()
                    .Select(message => message.TestCase)
                    .Where(message => message.TestMethod.TestClass.TestCollection.DisplayName == collectionName)
                    .ToList();
            }
        }

        public static IEnumerable<ITestCase> Find(this Xunit2Discoverer discoverer, Type type)
        {
            using (var sink = new SpyMessageSink<IDiscoveryCompleteMessage>())
            {
                discoverer.Find(type.FullName, false, sink, TestFrameworkOptions.ForDiscovery());
                sink.Finished.WaitOne();
                return sink.Messages.OfType<ITestCaseDiscoveryMessage>().Select(message => message.TestCase).ToList();
            }
        }
    }
}
