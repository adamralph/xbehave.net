// <copyright file="MethodCall.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    internal partial class MethodCall
    {
        private readonly IMethodInfo method;
        private readonly object[] arguments;

        public MethodCall(IMethodInfo method, IEnumerable<object> arguments)
        {
            this.method = method;
            this.arguments = (arguments ?? new object[0]).ToArray();
        }

        public IMethodInfo Method
        {
            get { return this.method; }
        }

        public IEnumerable<object> Arguments
        {
            get { return this.arguments.Select(x => x); }
        }

        public string Name
        {
            get
            {
                var paramStrings =
                    this.GetParameterNamesAndArguments().Select(pair => string.Concat(pair.Key, ": ", pair.Value == null ? "null" : pair.Value.ToString())).ToArray();

                var paramSuffix = paramStrings.Any() ? string.Concat("(", string.Join(", ", paramStrings), ")") : string.Empty;
                return string.Concat(this.method.Name, paramSuffix);
            }
        }

        private IEnumerable<KeyValuePair<string, object>> GetParameterNamesAndArguments()
        {
            var parameters = this.method.MethodInfo.GetParameters();
            for (var index = 0; index < Math.Max(parameters.Length, this.arguments.Length); ++index)
            {
                var parameterName = index < parameters.Length ? parameters[index].Name : "???";
                var argument = index < this.arguments.Length ? this.arguments[index] : null;
                yield return new KeyValuePair<string, object>(parameterName, argument);
            }
        }
    }
}