// <copyright file="IQueueTestCommandFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using Xunit.Sdk;

    internal interface IQueueTestCommandFactory
    {
        IEnumerable<ITestCommand> Create(Queue<Step> steps, IMethodInfo method);
    }
}