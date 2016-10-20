﻿// <copyright file="SpyMessageSink`1.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance.Infrastructure
{
    extern alias xunitexecution;

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Xunit.Abstractions;
    using xunitexecution.Xunit;

    public sealed class SpyMessageSink<TFinalMessage> : LongLivedMarshalByRefObject, IMessageSink, IDisposable
    {
        private readonly ManualResetEvent finished = new ManualResetEvent(initialState: false);
        private readonly IList<IMessageSinkMessage> messages = new List<IMessageSinkMessage>();

        public ManualResetEvent Finished
        {
            get { return this.finished; }
        }

        public IList<IMessageSinkMessage> Messages
        {
            get { return this.messages; }
        }

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

            return true;
        }
    }
}
