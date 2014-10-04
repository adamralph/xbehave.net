// <copyright file="SpyMessageSink.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Features.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Xunit;
    using Xunit.Abstractions;

    public class SpyMessageSink<TFinalMessage> : LongLivedMarshalByRefObject, IMessageSink
    {
        private readonly ManualResetEvent finished = new ManualResetEvent(initialState: false);
        private readonly List<IMessageSinkMessage> messages = new List<IMessageSinkMessage>();
        private readonly Func<IMessageSinkMessage, bool> cancellationThunk;

        public SpyMessageSink(Func<IMessageSinkMessage, bool> cancellationThunk = null)
        {
            this.cancellationThunk = cancellationThunk ?? (msg => true);
        }

        public ManualResetEvent Finished
        {
            get { return this.finished; }
        }

        public List<IMessageSinkMessage> Messages
        {
            get { return this.messages; }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Finished.Dispose();
        }

        public bool OnMessage(IMessageSinkMessage message)
        {
            this.Messages.Add(message);

            if (message is TFinalMessage)
            {
                this.Finished.Set();
            }

            return this.cancellationThunk(message);
        }
    }
}
