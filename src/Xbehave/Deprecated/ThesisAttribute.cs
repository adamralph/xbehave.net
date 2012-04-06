// <copyright file="ThesisAttribute.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using Xunit.Extensions;
    using Xunit.Sdk;

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
