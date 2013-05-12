// <copyright file="Command.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Xunit.Sdk;

    public class Command : TestCommand, ICommand
    {
        private readonly MethodCall methodCall;
        private readonly Argument[] arguments;
        private ShouldFailFastBeforeAttribute shouldFailFastAttribute;

        public Command(MethodCall methodCall)
            : base(methodCall == null ? null : methodCall.Method, null, methodCall == null ? 0 : MethodUtility.GetTimeoutParameter(methodCall.Method))
        {
            Guard.AgainstNullArgument("methodCall", methodCall);

            this.methodCall = methodCall;
            this.arguments = methodCall.Arguments.ToArray();
            this.DisplayName = GetString(methodCall.Method, this.arguments, methodCall.TypeArguments.ToArray());
            this.shouldFailFastAttribute = GetCustomAttribute<ShouldFailFastBeforeAttribute>(methodCall.Method);
        }

        public MethodCall MethodCall
        {
            get { return this.methodCall; }
        }

        // TODO (jamesfoster) make configurable
        protected static bool ShowExampleValues
        {
            get { return false; }
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

        private static T GetCustomAttribute<T>(IMethodInfo method) where T : Attribute
        {
            var attributeInfo = method.GetCustomAttributes(typeof(T)).FirstOrDefault();

            if (attributeInfo == null)
            {
                attributeInfo = method.Class.GetCustomAttributes(typeof(T)).FirstOrDefault();
            }

            if (attributeInfo == null)
            {
                return method.Class.Type.Assembly.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
            }
            
            return attributeInfo.GetInstance<T>();
        }

        private static string GetString(IMethodInfo method, Argument[] arguments, Type[] typeArguments)
        {
            var csharp = string.Concat(method.TypeName, ".", method.Name);
            if (typeArguments.Length > 0)
            {
                csharp = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}<{1}>",
                    csharp,
                    string.Join(", ", typeArguments.Select(typeArgument => GetString(typeArgument)).ToArray()));
            }

            var format = "{0}";
            var parameterTokens = new List<string>();
            if (Command.ShowExampleValues)
            {
                format += "({1})";

                var parameters = method.MethodInfo.GetParameters();
                int parameterIndex;
                for (parameterIndex = 0; parameterIndex < arguments.Length; parameterIndex++)
                {
                    if (arguments[parameterIndex].IsGeneratedDefault)
                    {
                        continue;
                    }

                    parameterTokens.Add(string.Concat(
                        parameterIndex >= parameters.Length ? "???" : parameters[parameterIndex].Name,
                        ": ",
                        GetString(arguments[parameterIndex])));
                }

                for (; parameterIndex < parameters.Length; parameterIndex++)
                {
                    parameterTokens.Add(parameters[parameterIndex].Name + ": ???");
                }
            }

            return string.Format(CultureInfo.InvariantCulture, format, csharp, string.Join(", ", parameterTokens.ToArray()));
        }

        private static string GetString(Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            var genericArgumentCSharpNames = type.GetGenericArguments().Select(typeArgument => GetString(typeArgument)).ToArray();
            return string.Concat(type.Name.Substring(0, type.Name.IndexOf('`')), "<", string.Join(", ", genericArgumentCSharpNames), ">");
        }

        private static string GetString(Argument argument)
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