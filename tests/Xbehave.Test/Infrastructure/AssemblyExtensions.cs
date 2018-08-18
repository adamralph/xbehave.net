namespace Xbehave.Test.Infrastructure
{
    using System;
    using System.IO;
    using System.Reflection;
    using LiteGuard;

    internal static class AssemblyExtensions
    {
        public static string GetLocalCodeBase(this Assembly assembly)
        {
            Guard.AgainstNullArgument(nameof(assembly), assembly);
            Guard.AgainstNullArgumentProperty(nameof(assembly), nameof(assembly.CodeBase), assembly.CodeBase);

            if (!assembly.CodeBase.StartsWith("file:///", StringComparison.OrdinalIgnoreCase))
            {
                var message = $"Code base {assembly.CodeBase} in wrong format; must start with 'file:///' (case-insensitive).";

                throw new ArgumentException(message, "assembly");
            }

            var codeBase = assembly.CodeBase.Substring(8);
            return Path.DirectorySeparatorChar == '/'
                ? "/" + codeBase
                : codeBase.Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
