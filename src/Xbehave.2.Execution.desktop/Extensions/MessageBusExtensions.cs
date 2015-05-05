// <copyright file="MessageBusExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution.Extensions
{
    using System;
    using System.Threading;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public static class MessageBusExtensions
    {
        public static void Queue(
            this IMessageBus messageBus,
            ITest test,
            Func<ITest, IMessageSinkMessage> createTestResultMessage,
            CancellationTokenSource cancellationTokenSource)
        {
            Guard.AgainstNullArgument("messageBus", messageBus);
            Guard.AgainstNullArgument("createTestResultMessage", createTestResultMessage);
            Guard.AgainstNullArgument("cancellationTokenSource", cancellationTokenSource);

            if (!messageBus.QueueMessage(new TestStarting(test)))
            {
                cancellationTokenSource.Cancel();
            }
            else
            {
                if (!messageBus.QueueMessage(createTestResultMessage(test)))
                {
                    cancellationTokenSource.Cancel();
                }
            }

            if (!messageBus.QueueMessage(new TestFinished(test, 0, null)))
            {
                cancellationTokenSource.Cancel();
            }
        }
    }
}
