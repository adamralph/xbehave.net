// <copyright file="WhenDefinition.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using Xbehave.Internal;

    internal class WhenDefinition : IWhenDefinition
    {
        private readonly Step step;

        public WhenDefinition(Step step)
        {
            this.step = step;
        }

        public IStepDefinition WithTimeout(int millisecondsTimeout)
        {
            this.step.MillisecondsTimeout = millisecondsTimeout;
            return this;
        }
    }
}
