// <copyright file="IWhen.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// An action in a scenario.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Required for fluent syntax.")]
    public interface IWhen : IStep
    {
    }
}
