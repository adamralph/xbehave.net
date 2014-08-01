// <copyright file="TypeUtility.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution.Shims
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    internal static class TypeUtility
    {
        private static readonly ITypeInfo ObjectTypeInfo = Reflector.Wrap(typeof(object));

        public static string GetDisplayNameWithArguments(IMethodInfo method, string baseDisplayName, object[] arguments, ITypeInfo[] genericTypes)
        {
            baseDisplayName += ResolveGenericDisplay(genericTypes);

            if (arguments == null)
            {
                return baseDisplayName;
            }

            var parameterInfos = method.GetParameters().ToArray();
            var displayValues = new string[Math.Max(arguments.Length, parameterInfos.Length)];
            int idx;

            for (idx = 0; idx < arguments.Length; idx++)
            {
                displayValues[idx] = ParameterToDisplayValue(GetParameterName(parameterInfos, idx), arguments[idx]);
            }

            // Fill-in any missing parameters with "???"
            for (; idx < parameterInfos.Length; idx++)
            {
                displayValues[idx] = GetParameterName(parameterInfos, idx) + ": ???";
            }

            return string.Format(CultureInfo.CurrentCulture, "{0}({1})", baseDisplayName, string.Join(", ", displayValues));
        }

        public static ITypeInfo ResolveGenericType(ITypeInfo genericType, object[] parameters, IParameterInfo[] parameterInfos)
        {
            bool sawNullValue = false;
            ITypeInfo matchedType = null;

            for (int idx = 0; idx < parameterInfos.Length; ++idx)
            {
                var parameterType = parameterInfos[idx].ParameterType;
                if (parameterType.IsGenericParameter && parameterType.Name == genericType.Name)
                {
                    object parameterValue = parameters[idx];

                    if (parameterValue == null)
                    {
                        sawNullValue = true;
                    }
                    else if (matchedType == null)
                    {
                        matchedType = Reflector.Wrap(parameterValue.GetType());
                    }
                    else if (matchedType.Name != parameterValue.GetType().FullName)
                    {
                        return ObjectTypeInfo;
                    }
                }
            }

            if (matchedType == null)
            {
                return ObjectTypeInfo;
            }

            return sawNullValue && matchedType.IsValueType ? ObjectTypeInfo : matchedType;
        }

        public static ITypeInfo[] ResolveGenericTypes(IMethodInfo method, object[] parameters)
        {
            var genericTypes = method.GetGenericArguments().ToArray();
            var resolvedTypes = new ITypeInfo[genericTypes.Length];
            var parameterInfos = method.GetParameters().ToArray();

            for (int idx = 0; idx < genericTypes.Length; ++idx)
            {
                resolvedTypes[idx] = ResolveGenericType(genericTypes[idx], parameters, parameterInfos);
            }

            return resolvedTypes;
        }

        private static string ConvertToSimpleTypeName(ITypeInfo type)
        {
            var baseTypeName = type.Name;

            int backTickIdx = baseTypeName.IndexOf('`');
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

            for (int idx = 0; idx < genericTypes.Length; idx++)
            {
                simpleNames[idx] = ConvertToSimpleTypeName(genericTypes[idx]);
            }

            return string.Format("{0}<{1}>", baseTypeName, string.Join(", ", simpleNames));
        }

        private static string GetParameterName(IParameterInfo[] parameters, int index)
        {
            if (index >= parameters.Length)
            {
                return "???";
            }

            return parameters[index].Name;
        }

        private static string ParameterToDisplayValue(string parameterName, object parameterValue)
        {
            return string.Format("{0}: {1}", parameterName, ArgumentFormatter.Format(parameterValue));
        }

        private static string ResolveGenericDisplay(ITypeInfo[] genericTypes)
        {
            if (genericTypes == null || genericTypes.Length == 0)
            {
                return string.Empty;
            }

            var typeNames = new string[genericTypes.Length];
            for (var idx = 0; idx < genericTypes.Length; idx++)
            {
                typeNames[idx] = ConvertToSimpleTypeName(genericTypes[idx]);
            }

            return string.Format("<{0}>", string.Join(", ", typeNames));
        }
    }
}
