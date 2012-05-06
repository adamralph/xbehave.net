// <copyright file="CommandBase.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Globalization;
    using System.Linq;
    using Xunit.Sdk;

    internal abstract class CommandBase : TestCommand
    {
        protected CommandBase(MethodCall call, int? contextOrdinal, int commandOrdinal, string commandName)
            : base(call.Method, commandName, MethodUtility.GetTimeoutParameter(call.Method))
        {
            var provider = CultureInfo.InvariantCulture;
            var tokens = new[]
                {
                    call.ToString(),
                    contextOrdinal.HasValue ? string.Format(provider, "context {0} ", contextOrdinal.Value.ToString("D2", provider)) : null,
                    string.Format(provider, "test {0} ", commandOrdinal.ToString("D2", provider)),
                    commandName,
                };

            this.DisplayName = string.Join(", ", tokens.Where(token => token != null).ToArray());
        }
    }
}