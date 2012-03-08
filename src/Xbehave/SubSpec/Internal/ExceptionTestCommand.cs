// <copyright file="Core.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using Xunit.Sdk;

    internal class ExceptionTestCommand : ActionTestCommand
    {
        public ExceptionTestCommand(IMethodInfo method, Xunit.Assert.ThrowsDelegate action)
            : base(method, null, 0, () => action())
        {
        }

        public override bool ShouldCreateInstance
        {
            get { return false; }
        }
    }
}