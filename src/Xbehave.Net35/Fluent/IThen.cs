// <copyright file="IThen.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// An assertion of an expected outcome of a scenario.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Required for fluent syntax.")]
    public interface IThen : IStep
    {
    }
}
