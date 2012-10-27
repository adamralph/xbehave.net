// <copyright file="ExceptionCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using Xunit.Sdk;

    public class ExceptionCommand : Command
    {
        private readonly Exception exception;

        public ExceptionCommand(IMethodInfo method, Exception exception)
            : base(method)
        {
            this.exception = exception;
        }

        public override bool ShouldCreateInstance
        {
            get { return false; }
        }

        public override MethodResult Execute(object testClass)
        {
            return new FailedResult(this.testMethod, this.exception, this.DisplayName);
        }
    }
}