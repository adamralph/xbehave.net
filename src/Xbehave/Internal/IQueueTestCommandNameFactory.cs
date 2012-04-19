// <copyright file="IQueueTestCommandNameFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;

    internal interface IQueueTestCommandNameFactory
    {
        string CreateSharedContextCreationName(Queue<Step> steps);

        string CreateSharedContextTestName(Queue<Step> contextSteps, Queue<Step> testSteps);

        string CreateSharedContextDisposalName(Queue<Step> steps);

        string CreateIsolatedContextTestName(Queue<Step> steps);

        string CreateSkippedTestName(Queue<Step> steps);
    }
}
