// <copyright file="ThenDefinition.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using Xbehave.Internal;

    internal class ThenDefinition : IThenDefinition
    {
        private readonly Step step;

        public ThenDefinition(Step step)
        {
            this.step = step;
        }

        public IThenDefinition WithTimeout(int millisecondsTimeout)
        {
            this.step.MillisecondsTimeout = millisecondsTimeout;
            return this;
        }
    }
}
