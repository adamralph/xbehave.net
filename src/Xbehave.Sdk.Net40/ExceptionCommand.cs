// <copyright file="ExceptionCommand.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using Xunit.Sdk;

    public class ExceptionCommand : Command
    {
        private readonly Exception exception;

        public ExceptionCommand(MethodCall methodCall, Exception exception)
            : base(methodCall)
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