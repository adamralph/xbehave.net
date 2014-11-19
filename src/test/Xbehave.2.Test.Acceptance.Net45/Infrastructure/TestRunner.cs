// <copyright file="TestRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Xunit;
    using Xunit.Abstractions;

    internal static class TestRunner
    {
        public static IList<Result> Run(this Type feature)
        {
            using (var xunit2 = new Xunit2(new NullSourceInformationProvider(), feature.Assembly.GetLocalCodeBase()))
            {
                return xunit2.Run(xunit2.Find(feature));
            }
        }

        private static IList<ITestCase> Find(this Xunit2Discoverer xunit2, Type type)
        {
            using (var sink = new SpyMessageSink<IDiscoveryCompleteMessage>())
            {
                xunit2.Find(type.FullName, false, sink, new XunitDiscoveryOptions());
                sink.Finished.WaitOne();
                return sink.Messages.OfType<ITestCaseDiscoveryMessage>().Select(message => message.TestCase).ToList();
            }
        }

        private static IList<Result> Run(this Xunit2 xunit2, IEnumerable<ITestCase> testCases)
        {
            using (var sink = new SpyMessageSink<ITestAssemblyFinished>())
            {
                xunit2.Run(testCases, sink, new XunitExecutionOptions());
                sink.Finished.WaitOne();
                return sink.Messages.OfType<ITestResultMessage>().Select(Map).ToList();
            }
        }

        private static Result Map(ITestResultMessage result)
        {
            var pass = result as ITestPassed;
            if (pass != null)
            {
                return new Pass { DisplayName = pass.TestDisplayName };
            }

            var skip = result as ITestSkipped;
            if (skip != null)
            {
                return new Skip { DisplayName = skip.TestDisplayName, Reason = skip.Reason };
            }

            var fail = result as ITestFailed;
            if (fail != null)
            {
                return new Fail
                {
                    DisplayName = fail.TestDisplayName,
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
