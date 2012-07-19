// <copyright file="ITestCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    public interface ITestCommand
    {
        MethodResult Execute(object feature);
    }
}