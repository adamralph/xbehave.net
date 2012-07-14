// <copyright file="ExceptionCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using Xunit.Sdk;

    public class ExceptionCommand : TestCommand
    {
        private readonly Exception exception;

        public ExceptionCommand(IMethodInfo method, Exception exception)
            : base(method, null, 0)
        {
            this.exception = exception;
        }

        public override bool ShouldCreateInstance
        {
            get { return false; }
        }

        public override MethodResult Execute(object testClass)
        {
            ExceptionUtility.RethrowWithNoStackTraceLoss(this.exception);
            return null;
        }
    }
}