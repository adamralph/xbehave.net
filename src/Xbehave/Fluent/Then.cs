// <copyright file="Then.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    internal class Then : IThen
    {
        private readonly IStep step;

        public Then(IStep step)
        {
            this.step = step;
        }

        public IStep WithTimeout(int milliSecondsTimeout)
        {
            return this.step.WithTimeout(milliSecondsTimeout);
        }
    }
}
