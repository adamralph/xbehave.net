using System;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xbehave.Execution.Extensions
{
    internal static class MessageBusExtensions
    {
        public static void Queue(
            this IMessageBus messageBus,
            ITest test,
            Func<ITest, IMessageSinkMessage> createTestResultMessage,
            CancellationTokenSource cancellationTokenSource)
        {
            messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));

            if (!messageBus.QueueMessage(new TestStarting(test)))
            {
                cancellationTokenSource?.Cancel();
            }
            else
            {
                if (!messageBus.QueueMessage(createTestResultMessage?.Invoke(test)))
                {
                    cancellationTokenSource?.Cancel();
                }
            }

            if (!messageBus.QueueMessage(new TestFinished(test, 0, null)))
            {
                cancellationTokenSource?.Cancel();
            }
        }
    }
}
