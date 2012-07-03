// <copyright file="CommandBase.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System.Globalization;
    using Xunit.Sdk;

    internal abstract class CommandBase : TestCommand
    {
        private readonly string localName;

        protected CommandBase(ScenarioDefinition definition, int contextOrdinal, int commandOrdinal, string commandName)
            : base(definition.Method, commandName, MethodUtility.GetTimeoutParameter(definition.Method))
        {
            var provider = CultureInfo.InvariantCulture;

            this.localName = string.Format(
                provider,
                "[{0}.{1}] {2}",
                contextOrdinal.ToString("D2", provider),
                commandOrdinal.ToString("D2", provider),
                commandName);

            this.DisplayName = string.Concat(definition.ToString(), " ", this.localName);
        }

        protected string LocalName
        {
            get { return this.localName; }
        }
    }
}