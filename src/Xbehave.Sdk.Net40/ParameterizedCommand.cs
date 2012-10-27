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
        private readonly Argument[] arguments;
        private readonly Type[] typeArguments;

        public ParameterizedCommand(IMethodInfo scenarioMethod)
            : this(scenarioMethod, null, null)
        {
        }

        public ParameterizedCommand(IMethodInfo scenarioMethod, Argument[] arguments, Type[] typeArguments)
            : base(scenarioMethod, null, MethodUtility.GetTimeoutParameter(scenarioMethod))
        {
            this.arguments = arguments != null ? arguments.ToArray() : new Argument[0];
            this.typeArguments = typeArguments != null ? typeArguments.ToArray() : new Type[0];
            this.DisplayName = GetCSharpMethodCall(scenarioMethod, this.arguments, this.typeArguments);
        }

        public IEnumerable<Argument> Arguments
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
                    string.Format(CultureInfo.CurrentCulture, "Expected {0} arguments, got {1} arguments", parameters.Length, this.arguments.Length));
            }

            try
            {
                testMethod.Invoke(testClass, this.arguments.Select(argument => argument.Value).ToArray());
            }
            catch (TargetInvocationException ex)
            {
                ExceptionUtility.RethrowWithNoStackTraceLoss(ex.InnerException);
            }

            return new PassedResult(testMethod, this.DisplayName);
        }

        private static string GetCSharpMethodCall(IMethodInfo method, Argument[] arguments, Type[] typeArguments)
        {
            var csharp = string.Concat(method.TypeName, ".", method.Name);
            if (typeArguments.Length > 0)
            {
                csharp = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}<{1}>",
                    csharp,
                    string.Join(", ", typeArguments.Select(typeArgument => GetCSharpName(typeArgument)).ToArray()));
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

            var genericArgumentCSharpNames = type.GetGenericArguments().Select(typeArgument => GetCSharpName(typeArgument)).ToArray();
            return string.Concat(type.Name.Substring(0, type.Name.IndexOf('`')), "<", string.Join(", ", genericArgumentCSharpNames), ">");
        }

        private static string GetCSharpLiteral(Argument argument)
        {
            if (argument.Value == null)
            {
                return "null";
            }

            if (argument.Value is char)
            {
                return "'" + argument.Value + "'";
            }

            var stringArgument = argument.Value as string;
            if (stringArgument != null)
            {
                if (stringArgument.Length > 50)
                {
                    return string.Concat("\"", stringArgument.Substring(0, 50), "\"...");
                }

                return string.Concat("\"", stringArgument, "\"");
            }

            return Convert.ToString(argument.Value, CultureInfo.InvariantCulture);
        }
    }
}