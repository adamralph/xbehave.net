// <copyright file="Given.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    internal class Given : IGiven
    {
        private readonly IScenarioPrimitive spec;

        public Given(IScenarioPrimitive spec)
        {
            this.spec = spec;
        }

        public IScenarioPrimitive WithTimeout(int millisecondsTimeout)
        {
            return this.spec.WithTimeout(millisecondsTimeout);
        }
    }
}
