// <copyright file="CommandBase.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System.Globalization;
    using Xunit.Sdk;

    public abstract class CommandBase : TestCommand
    {
        private readonly string name;

        protected CommandBase(IMethodInfo method, string scenarioName, int contextOrdinal, int stepOrdinal, string stepName)
            : base(method, stepName, method.GetTimeoutParameter())
        {
            var provider = CultureInfo.InvariantCulture;
            this.name = string.Format(provider, "[{0}.{1}] {2}", contextOrdinal.ToString("D2", provider), stepOrdinal.ToString("D2", provider), stepName);
            this.DisplayName = string.Concat(scenarioName, " ", this.name);
        }

        protected string Name
        {
            get { return this.name; }
        }
    }
}