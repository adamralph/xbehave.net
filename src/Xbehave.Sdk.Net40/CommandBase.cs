// <copyright file="CommandBase.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System.Globalization;
    using Xunit.Sdk;
    using Guard = Xbehave.Sdk.Infrastructure.Guard;

    public abstract class CommandBase : TestCommand
    {
        private readonly string name;

        protected CommandBase(IMethodInfo method, ScenarioDefinition definition, int contextOrdinal, int commandOrdinal, string commandName)
            : base(method, commandName, MethodUtility.GetTimeoutParameter(method))
        {
            Guard.AgainstNullArgument("definition", definition);

            var provider = CultureInfo.InvariantCulture;

            this.name = string.Format(
                provider,
                "[{0}.{1}] {2}",
                contextOrdinal.ToString("D2", provider),
                commandOrdinal.ToString("D2", provider),
                commandName);

            this.DisplayName = string.Concat(definition.ToString(), " ", this.name);
        }

        protected string Name
        {
            get { return this.name; }
        }
    }
}