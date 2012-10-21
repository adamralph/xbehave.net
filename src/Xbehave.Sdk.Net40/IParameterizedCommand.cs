// <copyright file="IParameterizedCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using Xunit.Sdk;

    public interface IParameterizedCommand : ITestCommand
    {
        IEnumerable<object> Arguments { get; }

        IEnumerable<Type> TypeArguments { get; }
    }
}