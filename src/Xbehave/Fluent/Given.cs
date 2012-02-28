// <copyright file="Given.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    internal class Given : IGiven
    {
        private readonly ISpecificationPrimitive spec;

        public Given(ISpecificationPrimitive spec)
        {
            this.spec = spec;
        }

        public ISpecificationPrimitive WithTimeout(int timeoutMs)
        {
            return this.spec.WithTimeout(timeoutMs);
        }
    }
}
