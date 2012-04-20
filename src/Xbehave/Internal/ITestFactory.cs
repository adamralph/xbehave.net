// <copyright file="ITestFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using Xunit.Sdk;

    internal interface ITestFactory
    {
        IEnumerable<ITestCommand> Create(IEnumerable<Step> givens, IEnumerable<Step> whens, IEnumerable<Step> thens, IMethodInfo method);
    }
}