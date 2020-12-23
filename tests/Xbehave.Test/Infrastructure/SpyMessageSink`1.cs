using System;
using System.Collections.Generic;
using System.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xbehave.Test.Infrastructure
{
    public sealed class SpyMessageSink<TFinalMessage> : LongLivedMarshalByRefObject, IMessageSink, IDisposable
    {
        public ManualResetEventSlim Finished { get; } = new ManualResetEventSlim(initialState: false);

        public Queue<IMessageSinkMessage> Messages { get; } = new Queue<IMessageSinkMessage>();

        public void Dispose() => this.Finished.Dispose();

        public bool OnMessage(IMessageSinkMessage message)
        {
            if (this.Finished.IsSet)
            {
                return false;
            }

            this.Messages.Enqueue(message);

            if (message is TFinalMessage)
            {
                this.Finished.Set();
                return false;
            }

            return true;
        }
    }
}
