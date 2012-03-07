// <copyright file="ContextDelegate.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;

    /// <summary>
    /// This member is deprecated (was part of the original SubSpec API).
    /// </summary>
    /// <returns>An instance of <see cref="IDisposable"/>.</returns>
    [Obsolete("Use Given(Func<IDisposable>) instead.")]
    public delegate IDisposable ContextDelegate();
}