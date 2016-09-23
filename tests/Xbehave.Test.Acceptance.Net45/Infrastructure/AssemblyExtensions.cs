// <copyright file="AssemblyExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance.Infrastructure
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using LiteGuard;

    internal static class AssemblyExtensions
    {
        public static string GetLocalCodeBase(this Assembly assembly)
        {
            Guard.AgainstNullArgument("assembly", assembly);
            Guard.AgainstNullArgumentProperty("assembly", "Codebase", assembly.CodeBase);

            if (!assembly.CodeBase.StartsWith("file:///", StringComparison.OrdinalIgnoreCase))
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Code base {0} in wrong format; must start with 'file:///' (case-insensitive).",
                    assembly.CodeBase);

                throw new ArgumentException(message, "assembly");
            }

            var codeBase = assembly.CodeBase.Substring(8);
            return Path.DirectorySeparatorChar == '/'
                ? "/" + codeBase
                : codeBase.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
