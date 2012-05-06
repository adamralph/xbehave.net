// <copyright file="MethodCall.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Xunit.Sdk;

    internal partial class MethodCall
    {
        private readonly IMethodInfo method;
        private readonly object[] args;
        private string text;

        public MethodCall(IMethodInfo method, IEnumerable<object> args)
        {
            this.method = method;
            this.args = (args ?? new object[0]).ToArray();
        }

        public IMethodInfo Method
        {
            get { return this.method; }
        }

        public override string ToString()
        {
            if (this.text == null)
            {
                var builder = new StringBuilder();
                builder.Append(this.method.TypeName);
                builder.Append(".");
                builder.Append(this.method.Name);
                if (this.args.Any())
                {
                    builder.Append("(");
                    builder.Append(string.Join(", ", ToStrings(this.method.MethodInfo.GetParameters(), this.args).ToArray()));
                    builder.Append(")");
                }

                this.text = builder.ToString();
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