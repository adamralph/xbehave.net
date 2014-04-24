// <copyright file="AssemblyExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Features.Infrastructure
{
    using System;
    using System.IO;
    using System.Reflection;

    internal static class AssemblyExtensions
    {
        public static string GetLocalCodeBase(this Assembly assembly)
        {
            var codeBase = assembly.CodeBase;
            if (!codeBase.StartsWith("file:///"))
            {
                throw new ArgumentException(
                    string.Format("Code base {0} in wrong format; must start with file:///", codeBase), "assembly");
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
