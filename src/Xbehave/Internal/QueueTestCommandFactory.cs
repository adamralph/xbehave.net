// <copyright file="QueueTestCommandFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using Xunit.Sdk;

    internal class QueueTestCommandFactory : IQueueTestCommandFactory
    {
        private readonly IQueueTestCommandNameFactory nameFactory;

        public QueueTestCommandFactory(IQueueTestCommandNameFactory nameFactory)
        {
            this.nameFactory = nameFactory;
        }

        public IEnumerable<ITestCommand> Create(Queue<Step> steps, IMethodInfo method)
        {
            // TODO: actually write some code
            return new ITestCommand[0];
        }
    }
}