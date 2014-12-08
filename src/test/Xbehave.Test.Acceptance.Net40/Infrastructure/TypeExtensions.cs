// <copyright file="TypeExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance.Infrastructure
{
    using System;
#if V2
    using System.Collections.Generic;
#endif
    using System.Globalization;
    using System.Linq;
#if !V2
    using System.Threading;
#endif
#if V2
    using Xunit;
    using Xunit.Abstractions;
#endif
#if !V2
    using Xunit.Sdk;
#endif

    internal static class TypeExtensions
    {
#if V2
        public static Result[] RunScenarios(this Type feature)
        {
            using (var xunit2 = new Xunit2(new NullSourceInformationProvider(), feature.Assembly.GetLocalCodeBase()))
            {
                return xunit2.Run(xunit2.Find(feature));
            }
        }

        private static ITestCase[] Find(this Xunit2Discoverer xunit2, Type type)
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
#else
        public static Result[] RunScenarios(this Type feature)
        {
            var command = new TestClassCommand(feature);
            Result[] results = null;
            var thread = new Thread(() => results = TestClassCommandRunner
                .Execute(command, command.EnumerateTestMethods().ToList(), null, null)
                .Results
                .OfType<MethodResult>()
                .Select(Map)
                .ToArray());

            thread.Start();
            thread.Join();

            return results;
        }

        private static Result Map(MethodResult result)
        {
            var pass = result as PassedResult;
            if (pass != null)
            {
                return new Pass { DisplayName = pass.DisplayName };
            }

            var skip = result as SkipResult;
            if (skip != null)
            {
                return new Skip { DisplayName = skip.DisplayName, Reason = skip.Reason };
            }

            var fail = result as FailedResult;
            if (fail != null)
            {
                return new Fail
                {
                    DisplayName = fail.DisplayName,
                    Message = fail.Message,
                    ExceptionType = fail.ExceptionType,
                    StackTrace = fail.StackTrace,
                };
            }

            throw new InvalidOperationException(
                string.Format(CultureInfo.InvariantCulture, "Unknown method result type '{0}'.", result.GetType()));
        }
#endif
    }
}
