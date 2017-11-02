// <copyright file="TypeInfoExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution.Extensions
{
    using System.Linq;
    using Xunit.Abstractions;

    internal static class TypeInfoExtensions
    {
        public static string ToSimpleString(this ITypeInfo type)
        {
            Guard.AgainstNullArgument(nameof(type), type);

            var baseTypeName = type.Name;

            var backTickIdx = baseTypeName.IndexOf('`');
            if (backTickIdx >= 0)
            {
                baseTypeName = baseTypeName.Substring(0, backTickIdx);
            }

            var lastIndex = baseTypeName.LastIndexOf('.');
            if (lastIndex >= 0)
            {
                baseTypeName = baseTypeName.Substring(lastIndex + 1);
            }

            if (!type.IsGenericType)
            {
                return baseTypeName;
            }

            var genericArgumentNames = type
                .GetGenericArguments()
                .Select(genericArgument => ToSimpleString(genericArgument));

            return $"{baseTypeName}<{string.Join(", ", genericArgumentNames)}>";
        }
    }
}
