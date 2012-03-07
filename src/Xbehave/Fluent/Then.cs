// <copyright file="Then.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    internal class Then : IThen
    {
        private readonly IScenarioPrimitive spec;

        public Then(IScenarioPrimitive spec)
        {
            this.spec = spec;
        }

        public IScenarioPrimitive WithTimeout(int milliSecondsTimeout)
        {
            return this.spec.WithTimeout(milliSecondsTimeout);
        }
    }
}
