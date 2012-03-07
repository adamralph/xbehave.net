// <copyright file="When.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    internal class When : IWhen
    {
        private readonly IScenarioPrimitive spec;

        public When(IScenarioPrimitive spec)
        {
            this.spec = spec;
        }

        public IScenarioPrimitive WithTimeout(int millisecondsTimeout)
        {
            return this.spec.WithTimeout(millisecondsTimeout);
        }
    }
}
