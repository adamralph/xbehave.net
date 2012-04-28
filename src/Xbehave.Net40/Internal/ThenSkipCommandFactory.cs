// <copyright file="ThenSkipCommandFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using System.Linq;
    using Xbehave.Infra;
    using Xunit.Sdk;

    internal class ThenSkipCommandFactory : ICommandFactory
    {
        private readonly ICommandNameFactory nameFactory;

        public ThenSkipCommandFactory(ICommandNameFactory nameFactory)
        {
            this.nameFactory = nameFactory;
        }

        public IEnumerable<ITestCommand> Create(IEnumerable<Step> contextSteps, IEnumerable<Step> thens, IMethodInfo method)
        {
            var message = "Action is ThenSkip (instead of Then or ThenInIsolation).";
            return thens.Select(then => (ITestCommand)new SkipCommand(method, this.nameFactory.Create(contextSteps.Concat(then.AsEnumerable())), message));
        }
    }
}