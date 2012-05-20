// <copyright file="ICommandFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System.Collections.Generic;
    using Xunit.Sdk;

    internal interface ICommandFactory
    {
        IEnumerable<ITestCommand> Create(ScenarioDefinition definition, int? contextOrdinal, IEnumerable<Step> steps);
    }
}