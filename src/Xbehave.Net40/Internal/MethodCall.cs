// <copyright file="MethodCall.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Xbehave.Infra;
    using Xunit.Sdk;

    internal partial class MethodCall
    {
        private readonly IMethodInfo method;
        private readonly object[] args;
        private string text;

        public MethodCall(IMethodInfo method, IEnumerable<object> args)
        {
            Require.NotNull(args, "args");

            this.method = method;
            this.args = args.ToArray();
        }

        public IMethodInfo Method
        {
            get { return this.method; }
        }

        public override string ToString()
        {
            if (this.text == null)
            {
                var paramList = string.Join(", ", ToStrings(this.method.MethodInfo.GetParameters(), this.args).ToArray());
                var paramSuffix = paramList.Length == 0 ? null : string.Concat("(", paramList, ")");
                this.text = string.Format(CultureInfo.InvariantCulture, "{0}.{1}{2}", this.method.TypeName, this.method.Name, paramSuffix);
            }

            return this.text;
        }

        private static IEnumerable<string> ToStrings(ParameterInfo[] parameters, object[] args)
        {
            for (var i = 0; i < Math.Max(parameters.Length, args.Length); ++i)
            {
                yield return ToString(parameters.ElementAtOrDefault(i), args.ElementAtOrDefault(i));
            }
        }

        private static string ToString(ParameterInfo parameter, object arg)
        {
            return string.Concat(parameter == null ? "???" : parameter.Name, ": ", (arg ?? "null").ToString());
        }
    }
}