// <copyright file="ThenSkipTestCommandFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    internal class ThenSkipTestCommandFactory : ITestCommandFactory
    {
        private readonly ITestCommandNameFactory nameFactory;

        public ThenSkipTestCommandFactory(ITestCommandNameFactory nameFactory)
        {
            this.nameFactory = nameFactory;
        }

        public IEnumerable<ITestCommand> Create(Step given, Step when, IEnumerable<Step> thens, IMethodInfo method)
        {
            return thens.Select(step =>
                (ITestCommand)new SkipCommand(method, this.nameFactory.Create(given, when, step), "Action is ThenSkip (instead of Then or ThenInIsolation)."));
        }
    }
}