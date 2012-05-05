// <copyright file="IAgnosticCommandFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using Xunit.Sdk;

    internal interface IAgnosticCommandFactory
    {
        IEnumerable<ITestCommand> Create(IEnumerable<Step> steps, IMethodInfo method);
    }
}