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
            this.DisplayName = GetCSharpCall(testMethod, this.Arguments, genericTypes);
        }

        public object[] Arguments { get; protected set; }

        public override MethodResult Execute(object testClass)
        {
            var parameters = testMethod.MethodInfo.GetParameters();
            if (parameters.Length != this.Arguments.Length)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, "Expected {0} parameters, got {1} parameters", parameters.Length, this.Arguments.Length));
            }

            try
            {
                testMethod.Invoke(testClass, this.Arguments);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionUtility.RethrowWithNoStackTraceLoss(ex.InnerException);
            }

            return new PassedResult(testMethod, this.DisplayName);
        }

        private static string GetCSharpCall(IMethodInfo testMethod, object[] arguments, Type[] genericTypes)
        {
            var displayName = MethodUtility.GetDisplayName(testMethod);
            if (genericTypes != null && genericTypes.Length > 0)
            {
                displayName = string.Format(
                    "{0}<{1}>",
                    displayName,
                    string.Join(", ", genericTypes.Select(genericType => GetCSharpName(genericType)).ToArray()));
            }

            var parameters = testMethod.MethodInfo.GetParameters();
            var parameterTokens = new string[Math.Max(arguments.Length, parameters.Length)];
            int parameterIndex;
            for (parameterIndex = 0; parameterIndex < arguments.Length; parameterIndex++)
            {
                parameterTokens[parameterIndex] = string.Concat(
                    parameterIndex >= parameters.Length ? "???" : parameters[parameterIndex].Name,
                    ": ",
                    arguments[parameterIndex]);
            }

            for (; parameterIndex < parameters.Length; parameterIndex++)
            {
                parameterTokens[parameterIndex] = parameters[parameterIndex].Name + ": ???";
            }

            return string.Format("{0}({1})", displayName, string.Join(", ", parameterTokens));
        }

        private static string GetCSharpName(Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            var genericArgumentCSharpNames = type.GetGenericArguments().Select(genericType => GetCSharpName(genericType)).ToArray();
            return string.Concat(type.Name.Substring(0, type.Name.IndexOf('`')), "<", string.Join(", ", genericArgumentCSharpNames), ">");
        }

        private static string GetCSharpLiteral(object argument)
        {
            if (argument == null)
            {
                return "null";
            }

            if (argument is char)
            {
                return "'" + argument + "'";
            }

            var stringArgument = argument as string;
            if (stringArgument != null)
            {
                if (stringArgument.Length > 50)
                {
                    return string.Concat("\"", stringArgument.Substring(0, 50), "\"...");
                }

                return string.Concat("\"", stringArgument, "\"");
            }

            return Convert.ToString(argument, CultureInfo.InvariantCulture);
        }
    }
}