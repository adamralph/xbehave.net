// <copyright file="ICommand.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using Xunit.Sdk;

    public interface ICommand : ITestCommand
    {
        MethodCall MethodCall { get; }
    }
}