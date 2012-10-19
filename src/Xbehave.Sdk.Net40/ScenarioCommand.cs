// <copyright file="ScenarioCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Xunit.Sdk;

    public class ScenarioCommand : TestCommand
    {
        public ScenarioCommand(IMethodInfo testMethod, object[] parameters)
            : this(testMethod, parameters, null)
        {
        }

        public ScenarioCommand(IMethodInfo testMethod, object[] arguments, Type[] genericTypes)
            : base(testMethod, null, MethodUtility.GetTimeoutParameter(testMethod))
        {
            this.Arguments = arguments ?? new object[0];
            this.DisplayName = GetDisplayName(testMethod, this.Arguments, genericTypes);
        }

        public object[] Arguments { get; protected set; }

        public override MethodResult Execute(object testClass)
        {
            try
            {
                ParameterInfo[] parameterInfos = testMethod.MethodInfo.GetParameters();
                if (parameterInfos.Length != this.Arguments.Length)
                {
                    throw new InvalidOperationException(string.Format("Expected {0} parameters, got {1} parameters", parameterInfos.Length, this.Arguments.Length));
                }

                testMethod.Invoke(testClass, this.Arguments);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionUtility.RethrowWithNoStackTraceLoss(ex.InnerException);
            }

            return new PassedResult(testMethod, DisplayName);
        }

        private static string GetDisplayName(IMethodInfo testMethod, object[] arguments, Type[] genericTypes)
        {
            var displayName = MethodUtility.GetDisplayName(testMethod);
            if (genericTypes != null && genericTypes.Length > 0)
            {
                displayName = string.Format(
                    "{0}<{1}>",
                    displayName,
                    string.Join(", ", genericTypes.Select(genericType => ConvertToSimpleTypeName(genericType)).ToArray()));
            }

            var parameters = testMethod.MethodInfo.GetParameters();
            var parameterTokens = new string[Math.Max(arguments.Length, parameters.Length)];
            int parameterIndex;
            for (parameterIndex = 0; parameterIndex < arguments.Length; parameterIndex++)
            {
                parameterTokens[parameterIndex] =
                    ParameterToDisplayValue(GetParameterName(parameters, parameterIndex), arguments[parameterIndex]);
            }

            for (; parameterIndex < parameters.Length; parameterIndex++)
            {
                parameterTokens[parameterIndex] = parameters[parameterIndex].Name + ": ???";
            }

            return string.Format("{0}({1})", displayName, string.Join(", ", parameterTokens));
        }

        private static string ConvertToSimpleTypeName(Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            Type[] genericTypes = type.GetGenericArguments();
            string[] simpleNames = new string[genericTypes.Length];

            for (int idx = 0; idx < genericTypes.Length; idx++)
            {
                simpleNames[idx] = ConvertToSimpleTypeName(genericTypes[idx]);
            }

            string baseTypeName = type.Name;
            int backTickIdx = type.Name.IndexOf('`');

            return baseTypeName.Substring(0, backTickIdx) + "<" + string.Join(", ", simpleNames) + ">";
        }

        private static string GetParameterName(ParameterInfo[] parameters, int index)
        {
            if (index >= parameters.Length)
            {
                return "???";
            }

            return parameters[index].Name;
        }

        private static string ParameterToDisplayValue(object parameterValue)
        {
            if (parameterValue == null)
            {
                return "null";
            }

            if (parameterValue is char)
            {
                return "'" + parameterValue + "'";
            }

            string stringParameter = parameterValue as string;
            if (stringParameter != null)
            {
                if (stringParameter.Length > 50)
                {
                    return "\"" + stringParameter.Substring(0, 50) + "\"...";
                }

                return "\"" + stringParameter + "\"";
            }

            return Convert.ToString(parameterValue, CultureInfo.InvariantCulture);
        }

        private static string ParameterToDisplayValue(string parameterName, object parameterValue)
        {
            return parameterName + ": " + ParameterToDisplayValue(parameterValue);
        }
    }
}