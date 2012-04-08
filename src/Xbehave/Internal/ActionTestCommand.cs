// <copyright file="ActionTestCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using Xunit.Sdk;

    internal class ActionTestCommand : TestCommand, ITestCommand
    {
        private readonly Action action;

        public ActionTestCommand(IMethodInfo method, string name, int timeout, Action action)
            : base(method, name, timeout)
        {
            this.action = action;
        }

        public override MethodResult Execute(object testClass)
        {
            this.action();
            return new PassedResult(this.testMethod, this.DisplayName);
        }
    }
}