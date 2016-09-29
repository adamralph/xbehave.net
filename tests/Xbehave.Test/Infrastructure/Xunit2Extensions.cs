// <copyright file="Xunit2Extensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using LiteGuard;
    using Xunit;
    using Xunit.Abstractions;

    public static class Xunit2Extensions
    {
        public static IEnumerable<IMessageSinkMessage> Run(this Xunit2 runner, IEnumerable<ITestCase> testCases)
        {
            Guard.AgainstNullArgument("runner", runner);

            if (!testCases.Any())
            {
                return Enumerable.Empty<IMessageSinkMessage>();
            }

            using (var sink = new SpyMessageSink<ITestCollectionFinished>())
            {
                runner.RunTests(testCases, sink, TestFrameworkOptions.ForExecution());
                sink.Finished.WaitOne();
                return sink.Messages.Select(_ => _);
            }
        }
    }
}
