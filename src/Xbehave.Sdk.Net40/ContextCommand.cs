// <copyright file="ContextCommand.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Globalization;

    [CLSCompliant(false)]
    public abstract class ContextCommand : Command
    {
        protected ContextCommand(MethodCall methodCall, int contextOrdinal, int commandOrdinal, bool omitArgumentsFromScenarioNames)
            : base(methodCall, omitArgumentsFromScenarioNames)
        {
            Guard.AgainstNullArgument("methodCall", methodCall);

            var provider = CultureInfo.InvariantCulture;

            this.Name = string.Format(
                provider,
                "[{0}.{1}.{2}]",
                methodCall.Ordinal.ToString("D2", provider),
                contextOrdinal.ToString("D2", provider),
                commandOrdinal.ToString("D2", provider));

            this.DisplayName = string.Format(provider, "{0} {1}", this.DisplayName, this.Name);
        }

        public string Name { get; protected set; }
    }
}
