// <copyright file="ThenSkipTestFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    internal class ThenSkipTestFactory : ITestFactory
    {
        private readonly ITestNameFactory nameFactory;

        public ThenSkipTestFactory(ITestNameFactory nameFactory)
        {
            this.nameFactory = nameFactory;
        }

        public IEnumerable<ITestCommand> Create(IEnumerable<Step> givens, IEnumerable<Step> whens, IEnumerable<Step> thens, IMethodInfo method)
        {
            return thens.Select(then =>
                (ITestCommand)new SkipCommand(method, this.nameFactory.Create(givens, whens, then), "Action is ThenSkip (instead of Then or ThenInIsolation)."));
        }
    }
}