// <copyright file="Argument.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;

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
    }
}
