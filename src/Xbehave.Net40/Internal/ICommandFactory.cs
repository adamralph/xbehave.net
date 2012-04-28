// <copyright file="ICommandFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using Xunit.Sdk;

    internal interface ICommandFactory
    {
        IEnumerable<ITestCommand> Create(IEnumerable<Step> contextSteps, IEnumerable<Step> thens, IMethodInfo method);
    }
}