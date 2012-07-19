// <copyright file="IMethodInfo.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    public interface IMethodInfo
    {
        System.Reflection.MethodInfo MethodInfo { get; }

        string Name { get; }

        string TypeName { get; }
    }
}