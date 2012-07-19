// <copyright file="MethodInfoWrapper.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Xunit
{
    public class MethodInfoWrapper : IMethodInfo
    {
        private readonly global::Xunit.Sdk.IMethodInfo method;

        public MethodInfoWrapper(global::Xunit.Sdk.IMethodInfo method)
        {
            this.method = method;
        }

        public System.Reflection.MethodInfo MethodInfo
        {
            get { return this.method.MethodInfo; }
        }

        public string Name
        {
            get { return this.method.Name; }
        }

        public string TypeName
        {
            get { return this.method.TypeName; }
        }
    }
}