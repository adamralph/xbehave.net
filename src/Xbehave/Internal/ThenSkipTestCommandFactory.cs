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
        private readonly TestCommandNameFactory nameFactory;

        public ThenSkipTestCommandFactory(TestCommandNameFactory nameFactory)
        {
            this.nameFactory = nameFactory;
        }

        public IEnumerable<ITestCommand> Create(DisposableStep given, Step when, IEnumerable<Step> thens, IMethodInfo method)
        {
            return thens.Select(step =>
                (ITestCommand)new SkipCommand(method, this.nameFactory.Create(given, when, step), "Action is ThenSkip (instead of Then or ThenInIsolation)."));
        }
    }
}