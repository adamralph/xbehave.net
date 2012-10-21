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
        protected Command(IMethodInfo method, object[] arguments, Type[] typeArguments, int contextOrdinal, int stepOrdinal)
            : base(method, arguments, typeArguments)
        {
            var provider = CultureInfo.InvariantCulture;
            this.Name = string.Format(provider, "[{0}.{1}]", contextOrdinal.ToString("D2", provider), stepOrdinal.ToString("D2", provider));
            this.DisplayName = string.Format(provider, "{0} {1}", this.DisplayName, this.Name);
        }

        public string Name { get; protected set; }
    }
}
