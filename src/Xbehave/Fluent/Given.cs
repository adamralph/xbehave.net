// <copyright file="Given.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    internal class Given : IGiven
    {
        private readonly IStep step;

        public Given(IStep step)
        {
            this.step = step;
        }

        public IStep WithTimeout(int millisecondsTimeout)
        {
            return this.step.WithTimeout(millisecondsTimeout);
        }
    }
}
