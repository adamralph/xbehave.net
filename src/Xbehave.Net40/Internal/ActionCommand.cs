// <copyright file="ActionCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using Xunit.Sdk;

    internal class ActionCommand : TestCommand
    {
        private readonly Action test;

        public ActionCommand(IMethodInfo method, string displayName, int timeout, Action test)
            : base(method, displayName, timeout)
        {
            this.test = test;
        }

        public override MethodResult Execute(object testClass)
        {
            this.test();
            return new PassedResult(this.testMethod, this.DisplayName);
        }
    }
}