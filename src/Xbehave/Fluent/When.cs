// <copyright file="When.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    internal class When : IWhen
    {
        private readonly IStep step;

        public When(IStep step)
        {
            this.step = step;
        }

        public IStep WithTimeout(int millisecondsTimeout)
        {
            return this.step.WithTimeout(millisecondsTimeout);
        }
    }
}
