// <copyright file="OmitArgumentsFromScenarioNamesAttribute.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;

    /// <summary>
    /// Omits arguments passed to scenario methods (e.g. using <see cref="ExampleAttribute"/>) from scenario names in test output.
    /// Disabled by default.
    /// This attribute can be applied at the level of method (scenario), class (feature) or assembly (product).
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class OmitArgumentsFromScenarioNamesAttribute : Attribute
    {
        private readonly bool enabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="OmitArgumentsFromScenarioNamesAttribute" /> class.
        /// </summary>
        /// <param name="enabled">If set to <c>true</c> [enabled].</param>
        public OmitArgumentsFromScenarioNamesAttribute(bool enabled)
        {
            this.enabled = enabled;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="OmitArgumentsFromScenarioNamesAttribute"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled
        {
            get { return this.enabled; }
        }
    }
}
