// <copyright file="TypeExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Xunit.Sdk;

    internal static class TypeExtensions
    {
        public static IList<Result> RunScenarios(this Type feature)
        {
            var command = new TestClassCommand(feature);
            List<Result> results = null;
            var thread = new Thread(() => results = TestClassCommandRunner
                .Execute(command, command.EnumerateTestMethods().ToList(), null, null)
                .Results
                .OfType<MethodResult>()
                .Select(Map)
                .ToList());

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
    }
}
