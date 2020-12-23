using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Xbehave.Test.Infrastructure
{
    public static class Xunit2Extensions
    {
        public static Queue<IMessageSinkMessage> Run(this Xunit2 runner, IEnumerable<ITestCase> testCases, TestAssemblyConfiguration testAssemblyConfiguration)
        {
            if (!testCases.Any())
            {
                return new Queue<IMessageSinkMessage>();
            }

            using (var sink = new SpyMessageSink<ITestCollectionFinished>())
            {
                runner.RunTests(testCases, sink, TestFrameworkOptions.ForExecution(testAssemblyConfiguration));
                sink.Finished.Wait();
                return sink.Messages;
            }
        }
    }
}
