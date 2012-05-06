// <copyright file="CommandBase.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Globalization;
    using Xunit.Sdk;

    internal abstract class CommandBase : TestCommand
    {
        protected CommandBase(MethodCall call, int ordinal, string name, string context)
            : base(call.Method, CreateCommandName(call, ordinal, name, context), MethodUtility.GetTimeoutParameter(call.Method))
        {
        }

        private static string CreateCommandName(MethodCall call, int ordinal, string name, string context)
        {
            return string.Concat(
                call.ToString(),
                context == null ? null : "(" + context + ")",
                ".",
                ordinal.ToString("D2", CultureInfo.InvariantCulture),
                ".",
                "\"",
                name,
                "\"");
        }
    }
}