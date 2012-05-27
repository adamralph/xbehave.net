// <copyright file="ScenarioDefinition.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Xbehave.Sdk.Infra;
    using Xunit.Sdk;

    internal partial class ScenarioDefinition
    {
        private readonly Action body;
        private readonly IMethodInfo method;
        private readonly object[] args;

        private string text;

        public ScenarioDefinition(IMethodInfo method, IEnumerable<object> args, Action body)
        {
            Require.NotNull(body, "body");
            Require.NotNull(method, "method");
            Require.NotNull(args, "args");

            this.body = body;
            this.method = method;
            this.args = args.ToArray();
        }

        public IMethodInfo Method
        {
            get { return this.method; }
        }

        public object[] Args
        {
            get { return this.args; }
        }

        public void Execute()
        {
            this.body.Invoke();
        }

        public override string ToString()
        {
            if (this.text == null)
            {
                string parameterSuffix = null;

                var parameters = this.method.MethodInfo.GetParameters();
                if (parameters.Length > 0 || this.args.Length > 0)
                {
                    var tokens = new List<string>();
                    for (var i = 0; i < Math.Max(parameters.Length, this.args.Length); ++i)
                    {
                        var parameter = parameters.ElementAtOrDefault(i);
                        var arg = this.args.ElementAtOrDefault(i);
                        tokens.Add(string.Concat(parameter == null ? "???" : parameter.Name, ": ", (arg ?? "null").ToString()));
                    }

                    parameterSuffix = string.Concat("(", string.Join(", ", tokens.ToArray()), ")");
                }

                this.text = string.Format(CultureInfo.InvariantCulture, "{0}.{1}{2}", this.method.TypeName, this.method.Name, parameterSuffix);
            }

            return this.text;
        }
    }
}