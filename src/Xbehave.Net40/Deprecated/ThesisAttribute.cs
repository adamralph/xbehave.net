// <copyright file="ThesisAttribute.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;

    /// <summary>
    /// This member is deprecated.
    /// </summary>
    [Obsolete("Use ScenarioAttribute instead.")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [CLSCompliant(false)]
    public class ThesisAttribute : ScenarioAttribute
    {
    }
}
