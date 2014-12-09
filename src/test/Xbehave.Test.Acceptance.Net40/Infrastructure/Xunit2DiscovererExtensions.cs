// <copyright file="Xunit2DiscovererExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if V2
namespace Xbehave.Test.Acceptance.Infrastructure
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
                discoverer.Find(false, sink, new XunitDiscoveryOptions());
                sink.Finished.WaitOne();
                return sink.Messages.OfType<ITestCaseDiscoveryMessage>()
                    .Select(message => message.TestCase)
                    .Where(message => message.TestMethod.TestClass.TestCollection.DisplayName == collectionName)
                    .ToArray();
            }
        }

        public static IEnumerable<ITestCase> Find(this Xunit2Discoverer discoverer, Type type)
        {
            using (var sink = new SpyMessageSink<IDiscoveryCompleteMessage>())
            {
                discoverer.Find(type.FullName, false, sink, new XunitDiscoveryOptions());
                sink.Finished.WaitOne();
                return sink.Messages.OfType<ITestCaseDiscoveryMessage>().Select(message => message.TestCase).ToArray();
            }
        }
    }
}
#endif
