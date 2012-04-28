// <copyright file="IGivenDefinition.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The definition of an arrangement for a scenario.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Required for fluent syntax.")]
    public interface IGivenDefinition : IStepDefinition
    {
    }
}
