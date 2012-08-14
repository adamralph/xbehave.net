// <copyright file="Command.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Xunit.Extensions;
    using Xunit.Sdk;

    [CLSCompliant(false)]
    public abstract class Command : TheoryCommand
    {
        protected Command(IMethodInfo method, object[] args, int contextOrdinal, int stepOrdinal)
            : base(method, args, ResolveTypeArguments(method, args))
        {
            var provider = CultureInfo.InvariantCulture;
            this.Name = string.Format(provider, "[{0}.{1}]", contextOrdinal.ToString("D2", provider), stepOrdinal.ToString("D2", provider));
            this.DisplayName = string.Format(provider, "{0} {1}", this.DisplayName, this.Name);
        }

        public string Name { get; protected set; }

        private static Type[] ResolveTypeArguments(IMethodInfo genericMethodDefinition, object[] arguments)
        {
            if (genericMethodDefinition == null || genericMethodDefinition.MethodInfo == null || arguments == null)
            {
                return null;
            }

            var genericParameters = genericMethodDefinition.MethodInfo.GetParameters();
            return genericMethodDefinition.MethodInfo.GetGenericArguments()
                .Select(typeParameter => ResolveTypeArgument(typeParameter, genericParameters, arguments)).ToArray();
        }

        private static Type ResolveTypeArgument(Type typeParameter, ParameterInfo[] genericParameters, object[] arguments)
        {
            Type typeArgument = null;
            for (var index = 0; index < genericParameters.Length; ++index)
            {
                if (genericParameters[index].ParameterType != typeParameter)
                {
                    continue;
                }

                var argument = arguments[index];
                if (argument == null)
                {
                    continue;
                }

                if (typeArgument == null)
                {
                    typeArgument = argument.GetType();
                }
                else if (typeArgument != argument.GetType())
                {
                    return typeof(object);
                }
            }

            return typeArgument ?? typeof(object);
        }
    }
}
