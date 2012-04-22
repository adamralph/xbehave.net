// <copyright file="Given.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using Xbehave.Internal;

    internal class Given : IGiven
    {
        private readonly Step step;

        public Given(Step step)
        {
            this.step = step;
        }

        public IStep WithTimeout(int millisecondsTimeout)
        {
            this.step.MillisecondsTimeout = millisecondsTimeout;
            return this;
        }
    }
}
