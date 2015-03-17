// <copyright file="Argument.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Reflection;

    public class Argument
    {
        private static readonly MethodInfo genericFactoryMethod = CreateGenericFactoryMethod();

        private readonly object value;
        private readonly bool isGeneratedDefault;

        public Argument(Type type)
        {
            Guard.AgainstNullArgument("type", type);

            this.value = genericFactoryMethod.MakeGenericMethod(type).Invoke(null, null);
            this.isGeneratedDefault = true;
        }

        public Argument(object value)
        {
            this.value = value;
        }

        public object Value
        {
            get { return this.value; }
        }

        public bool IsGeneratedDefault
        {
            get { return this.isGeneratedDefault; }
        }

        public override string ToString()
        {
            if (this.Value == null)
            {
                return "null";
            }

            if (this.Value is char)
            {
                return "'" + this.Value + "'";
            }

            var stringArgument = this.Value as string;
            if (stringArgument != null)
            {
                return stringArgument.Length > 50
                    ? string.Concat("\"", stringArgument.Substring(0, 50), "\"...")
                    : string.Concat("\"", stringArgument, "\"");
            }

            return Convert.ToString(this.Value, CultureInfo.InvariantCulture);
        }

        private static MethodInfo CreateGenericFactoryMethod()
        {
            Expression<Func<object>> template = () => CreateDefault<object>();
            return ((MethodCallExpression)template.Body).Method.GetGenericMethodDefinition();
        }

        private static T CreateDefault<T>()
        {
            return default(T);
        }
    }
}
