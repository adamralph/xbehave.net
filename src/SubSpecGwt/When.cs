// <copyright file="When.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace SubSpec
{
    internal class When : IWhen
    {
        private readonly ISpecificationPrimitive spec;

        public When(ISpecificationPrimitive spec)
        {
            this.spec = spec;
        }

        public ISpecificationPrimitive WithTimeout(int timeoutMs)
        {
            return this.spec.WithTimeout(timeoutMs);
        }
    }
}
