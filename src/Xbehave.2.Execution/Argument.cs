// <copyright file="Argument.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Globalization;

    public class Argument
    {
        private readonly object value;
        private readonly bool isGeneratedDefault;

        public Argument(Type type)
        {
            Guard.AgainstNullArgument("type", type);

            this.value = type.IsValueType ? Activator.CreateInstance(type) : null;
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
                if (stringArgument.Length > 50)
                {
                    return string.Concat("\"", stringArgument.Substring(0, 50), "\"...");
                }

                return string.Concat("\"", stringArgument, "\"");
            }

            return Convert.ToString(this.Value, CultureInfo.InvariantCulture);
        }
    }
}
