// <copyright file="ExceptionTestCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using Xunit.Sdk;

    internal class ExceptionTestCommand : ActionTestCommand
    {
        public ExceptionTestCommand(IMethodInfo method, Action action)
            : base(method, null, 0, action)
        {
        }

        public override bool ShouldCreateInstance
        {
            get { return false; }
        }
    }
}