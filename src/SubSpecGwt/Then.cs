// <copyright file="Then.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace SubSpec
{
    internal class Then : IThen
    {
        private readonly ISpecificationPrimitive spec;

        public Then(ISpecificationPrimitive spec)
        {
            this.spec = spec;
        }

        public ISpecificationPrimitive WithTimeout(int timeoutMs)
        {
            return this.spec.WithTimeout(timeoutMs);
        }
    }
}
