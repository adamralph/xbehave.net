// <copyright file="AgnosticCommandFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Xbehave.Infra;
    using Xunit.Sdk;

    internal class AgnosticCommandFactory : IAgnosticCommandFactory
    {
        private readonly IDisposer disposer;

        public AgnosticCommandFactory(IDisposer disposer)
        {
            this.disposer = disposer;
        }

        public IEnumerable<ITestCommand> Create(IEnumerable<Step> steps, IMethodInfo method)
        {
            throw new NotImplementedException();
        }
    }
}