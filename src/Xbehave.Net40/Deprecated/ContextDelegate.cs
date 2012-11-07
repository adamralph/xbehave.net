// <copyright file="ContextDelegate.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;

    /// <summary>
    /// This member is deprecated (was part of the original SubSpec API).
    /// </summary>
    /// <returns>An instance of <see cref="IDisposable"/>.</returns>
    [Obsolete("Use Given(Action body, Action teardown) instead.")]
    public delegate IDisposable ContextDelegate();
}