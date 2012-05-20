// <copyright file="CommandBase.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System.Globalization;
    using Xunit.Sdk;

    internal abstract class CommandBase : TestCommand
    {
        protected CommandBase(ScenarioDefinition definition, int? contextOrdinal, int commandOrdinal, string commandName)
            : base(definition.Method, commandName, MethodUtility.GetTimeoutParameter(definition.Method))
        {
            var provider = CultureInfo.InvariantCulture;

            this.DisplayName = string.Format(
                provider,
                "{0} [{1}{2}] {3}",
                definition.ToString(),
                contextOrdinal.HasValue ? string.Format(provider, "context {0}, ", contextOrdinal.Value.ToString("D2", provider)) : null,
                string.Format(provider, "test {0}", commandOrdinal.ToString("D2", provider)),
                commandName);
        }
    }
}