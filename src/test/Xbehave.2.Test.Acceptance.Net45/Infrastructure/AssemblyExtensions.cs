// <copyright file="AssemblyExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance.Infrastructure
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;

    internal static class AssemblyExtensions
    {
        public static string GetLocalCodeBase(this Assembly assembly)
        {
            var codeBase = assembly.CodeBase;
            if (!codeBase.StartsWith("file:///", StringComparison.OrdinalIgnoreCase))
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Code base {0} in wrong format; must start with 'file:///' (case-insensitive).",
                    codeBase);

                throw new ArgumentException(message, "assembly");
            }

            codeBase = codeBase.Substring(8);
            if (Path.DirectorySeparatorChar == '/')
            {
                return "/" + codeBase;
            }

            return codeBase.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
