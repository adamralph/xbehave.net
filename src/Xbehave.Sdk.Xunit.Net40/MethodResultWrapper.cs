// <copyright file="MethodResultWrapper.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Xunit
{
    public class MethodResultWrapper : MethodResult
    {
        private readonly global::Xunit.Sdk.MethodResult methodResult;

        public MethodResultWrapper(global::Xunit.Sdk.MethodResult methodResult)
        {
            this.methodResult = methodResult;
        }
    }
}