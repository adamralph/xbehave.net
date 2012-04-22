// <copyright file="When.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using Xbehave.Internal;

    internal class When : IWhen
    {
        private readonly Step step;

        public When(Step step)
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
