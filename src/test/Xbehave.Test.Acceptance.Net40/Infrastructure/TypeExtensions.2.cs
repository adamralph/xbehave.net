// <copyright file="TypeExtensions.2.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if V2
namespace Xbehave.Test.Acceptance.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Xunit;
    using Xunit.Abstractions;

    internal static partial class TypeExtensions
    {
        public static Result[] RunScenarios(this Assembly assembly, string collectionName)
        {
            using (var xunit2 = new Xunit2(new NullSourceInformationProvider(), assembly.GetLocalCodeBase()))
            {
                return xunit2.Run(xunit2.Find(collectionName));
            }
        }

        public static Result[] RunScenarios(this Type feature)
        {
            using (var xunit2 = new Xunit2(new NullSourceInformationProvider(), feature.Assembly.GetLocalCodeBase()))
            {
                return xunit2.Run(xunit2.Find(feature));
            }
        }

        private static IEnumerable<ITestCase> Find(this Xunit2Discoverer xunit2, string collectionName)
        {
            using (var sink = new SpyMessageSink<IDiscoveryCompleteMessage>())
            {
                xunit2.Find(false, sink, new XunitDiscoveryOptions());
                sink.Finished.WaitOne();
                return sink.Messages.OfType<ITestCaseDiscoveryMessage>()
                    .Select(message => message.TestCase)
                    .Where(message => message.TestMethod.TestClass.TestCollection.DisplayName == collectionName)
                    .ToArray();
            }
        }

        private static IEnumerable<ITestCase> Find(this Xunit2Discoverer xunit2, Type type)
        {
            using (var sink = new SpyMessageSink<IDiscoveryCompleteMessage>())
            {
                xunit2.Find(type.FullName, false, sink, new XunitDiscoveryOptions());
                sink.Finished.WaitOne();
                return sink.Messages.OfType<ITestCaseDiscoveryMessage>().Select(message => message.TestCase).ToArray();
            }
        }

        private static Result[] Run(this Xunit2 xunit2, IEnumerable<ITestCase> testCases)
        {
            using (var sink = new SpyMessageSink<ITestAssemblyFinished>())
            {
                xunit2.Run(testCases, sink, new XunitExecutionOptions());
                sink.Finished.WaitOne();
                return sink.Messages.OfType<ITestResultMessage>().Select(Map).ToArray();
            }
        }

        private static Result Map(ITestResultMessage result)
        {
            var pass = result as ITestPassed;
            if (pass != null)
            {
                return new Pass { DisplayName = pass.Test.DisplayName };
            }

            var skip = result as ITestSkipped;
            if (skip != null)
            {
                return new Skip { DisplayName = skip.Test.DisplayName, Reason = skip.Reason };
            }

            var fail = result as ITestFailed;
            if (fail != null)
            {
                return new Fail
                {
                    DisplayName = fail.Test.DisplayName,
                    Message = fail.Messages[0],
                    ExceptionType = fail.ExceptionTypes[0],
                    StackTrace = fail.StackTraces[0],
                };
            }

            throw new InvalidOperationException(
                string.Format(CultureInfo.InvariantCulture, "Unknown test result message type '{0}'.", result.GetType()));
        }
    }
}
#endif
