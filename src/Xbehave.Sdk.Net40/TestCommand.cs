// <copyright file="TestCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    public abstract class TestCommand : ITestCommand
    {
        private readonly IMethodInfo method;

        protected TestCommand(IMethodInfo method)
        {
            this.method = method;
        }

        protected IMethodInfo Method
        {
            get { return this.method; }
        }

        public abstract MethodResult Execute(object testClass);
    }
}