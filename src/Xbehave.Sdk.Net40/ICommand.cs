// <copyright file="ICommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using Xunit.Sdk;

    public interface ICommand : ITestCommand
    {
        MethodCall MethodCall { get; }
    }
}