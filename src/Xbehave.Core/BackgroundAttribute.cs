// <copyright file="BackgroundAttribute.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;
    using Xunit.Sdk;

    /// <summary>
    /// Applied to a method to indicate a background for each scenario defined in the same feature class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Designed for extensibility.")]
    public class BackgroundAttribute : Attribute
    {
    }
}
