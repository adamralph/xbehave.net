// <copyright file="ActionTestCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
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
            try
            {
                this.action();
                return new PassedResult(testMethod, DisplayName);
            }
            catch (Exception ex)
            {
                return new FailedResult(testMethod, ex, DisplayName);
            }
        }
    }
}