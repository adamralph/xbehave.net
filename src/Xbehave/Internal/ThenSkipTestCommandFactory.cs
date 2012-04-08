// <copyright file="ThenSkipTestCommandFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    internal class ThenSkipTestCommandFactory
    {
        public IEnumerable<ITestCommand> Create(DisposableStep given, Step when, IEnumerable<Step> thens, string name, IMethodInfo method)
        {
            return thens
                .Select(step => (ITestCommand)new SkipCommand(method, name + ", " + step.Message, "Action is ThenSkip (instead of Then or ThenInIsolation)"));
        }
    }
}