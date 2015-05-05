// <copyright file="TypeInfoExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution.Extensions
{
    using System.Globalization;
    using System.Linq;
    using Xunit.Abstractions;

    public static class TypeInfoExtensions
    {
        public static string ToSimpleString(this ITypeInfo type)
        {
            Guard.AgainstNullArgument("type", type);

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

            var genericTypes = type.GetGenericArguments().ToArray();
            var simpleNames = new string[genericTypes.Length];

            for (var idx = 0; idx < genericTypes.Length; idx++)
            {
                simpleNames[idx] = ToSimpleString(genericTypes[idx]);
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}<{1}>", baseTypeName, string.Join(", ", simpleNames));
        }
    }
}
