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
    using Xunit.Sdk;

    internal static class TestRunner
    {
        public static IEnumerable<Result> Run(Type featureDefinition)
        {
            var feature = new TestClassCommand(featureDefinition);
            MethodResult[] results = null;
            var thread = new Thread(() => results =
                TestClassCommandRunner.Execute(feature, feature.EnumerateTestMethods().ToList(), startCallback: null, resultCallback: null).Results
                .OfType<MethodResult>().ToArray());
            thread.Start();
            thread.Join();

            return results.Select(Map);
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

            throw new ArgumentException(
                string.Format(CultureInfo.InvariantCulture, "Unknown method result type '{0}'.", result.GetType()),
                "result");
        }
    }
}
