// <copyright file="ParameterizedCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Xunit.Sdk;

    public class ParameterizedCommand : TestCommand, IParameterizedCommand
    {
        private readonly object[] arguments;
        private readonly Type[] typeArguments;

        public ParameterizedCommand(IMethodInfo scenarioMethod)
            : this(scenarioMethod, null, null)
        {
        }

        public ParameterizedCommand(IMethodInfo scenarioMethod, object[] arguments, Type[] genericTypes)
            : base(scenarioMethod, null, MethodUtility.GetTimeoutParameter(scenarioMethod))
        {
            if (arguments != null)
            {
                this.arguments = arguments.ToArray();
            }
            else
            {
                this.arguments = new object[0];
            }

            if (genericTypes != null)
            {
                this.typeArguments = genericTypes.ToArray();
            }
            else
            {
                this.typeArguments = new Type[0];
            }

            this.DisplayName = GetCSharpMethodCall(scenarioMethod, this.arguments, genericTypes);
        }

        public IEnumerable<object> Arguments
        {
            get { return this.arguments.Select(argument => argument); }
        }

        public IEnumerable<Type> TypeArguments
        {
            get { return this.typeArguments.Select(typeArgument => typeArgument); }
        }

        public override MethodResult Execute(object testClass)
        {
            var parameters = testMethod.MethodInfo.GetParameters();
            if (parameters.Length != this.arguments.Length)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, "Expected {0} parameters, got {1} parameters", parameters.Length, this.arguments.Length));
            }

            try
            {
                testMethod.Invoke(testClass, this.arguments);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionUtility.RethrowWithNoStackTraceLoss(ex.InnerException);
            }

            return new PassedResult(testMethod, this.DisplayName);
        }

        private static string GetCSharpMethodCall(IMethodInfo method, object[] arguments, Type[] genericTypes)
        {
            var csharp = MethodUtility.GetDisplayName(method);
            if (genericTypes != null && genericTypes.Length > 0)
            {
                csharp = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}<{1}>",
                    csharp,
                    string.Join(", ", genericTypes.Select(genericType => GetCSharpName(genericType)).ToArray()));
            }

            var parameters = method.MethodInfo.GetParameters();
            var parameterTokens = new string[Math.Max(arguments.Length, parameters.Length)];
            int parameterIndex;
            for (parameterIndex = 0; parameterIndex < arguments.Length; parameterIndex++)
            {
                parameterTokens[parameterIndex] = string.Concat(
                    parameterIndex >= parameters.Length ? "???" : parameters[parameterIndex].Name,
                    ": ",
                    GetCSharpLiteral(arguments[parameterIndex]));
            }

            for (; parameterIndex < parameters.Length; parameterIndex++)
            {
                parameterTokens[parameterIndex] = parameters[parameterIndex].Name + ": ???";
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}({1})", csharp, string.Join(", ", parameterTokens));
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