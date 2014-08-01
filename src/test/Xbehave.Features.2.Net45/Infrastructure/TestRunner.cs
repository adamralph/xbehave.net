// <copyright file="TestRunner.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Xbehave.Features.Infrastructure;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    internal static class TestRunner
    {
        public static IEnumerable<Result> Run(Type featureDefinition)
        {
            using (var test = new AcceptanceTest())
            {
                List<ITestResultMessage> messages = null;
                var thread = new Thread(() => messages = test.Run<ITestResultMessage>(featureDefinition));
                thread.Start();
                thread.Join();
                return messages.Select(Map).ToArray();
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

            throw new ArgumentException(
                string.Format(CultureInfo.InvariantCulture, "Unknown test result message type '{0}'.", result.GetType()),
                "result");
        }
    }
}
