// <copyright file="TeardownCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Xunit.Sdk;

    [CLSCompliant(false)]
    public class TeardownCommand : ContextCommand
    {
        private readonly Action[] teardowns;

        public TeardownCommand(IMethodInfo method, Argument[] arguments, Type[] typeArguments, int contextOrdinal, int stepOrdinal, IEnumerable<Action> teardowns)
            : base(method, arguments, typeArguments, contextOrdinal, stepOrdinal)
        {
            Guard.AgainstNullArgument("teardowns", teardowns);

            this.teardowns = teardowns.ToArray();
            this.Name = string.Format(CultureInfo.InvariantCulture, "{0} {1}", this.Name, "(Teardown)");
            this.DisplayName = string.Format(CultureInfo.InvariantCulture, "{0} {1}", this.DisplayName, "(Teardown)");
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Simulating behaviour of nested using blocks.")]
        public override MethodResult Execute(object testClass)
        {
            Exception exception = null;
            foreach (var teardown in this.teardowns.Where(action => action != null))
            {
                try
                {
                    teardown.Invoke();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            return exception == null ? (MethodResult)new PassedResult(this.testMethod, this.DisplayName) : new FailedResult(this.testMethod, exception, this.DisplayName);
        }
    }
}
