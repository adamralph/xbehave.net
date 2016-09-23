// <copyright file="Xunit2Extensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance.Infrastructure
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

            using (var sink = new SpyMessageSink<ITestAssemblyFinished>())
            {
                runner.RunTests(testCases, sink, TestFrameworkOptions.ForExecution());
                sink.Finished.WaitOne();
                return sink.Messages.Select(message => message);
            }
        }
    }
}
