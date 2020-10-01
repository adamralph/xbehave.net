namespace Xbehave.Test.Infrastructure
{
    using System;
    using System.IO;
    using System.Reflection;

    internal static class AssemblyExtensions
    {
        public static string GetLocalCodeBase(this Assembly assembly)
        {
            if (!assembly.CodeBase.StartsWith("file:///", StringComparison.OrdinalIgnoreCase))
            {
                var message = $"Code base {assembly.CodeBase} in wrong format; must start with 'file:///' (case-insensitive).";

                throw new ArgumentException(message, nameof(assembly));
            }

            var codeBase = assembly.CodeBase.Substring(8);
            return Path.DirectorySeparatorChar == '/'
                ? "/" + codeBase
                : codeBase.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
