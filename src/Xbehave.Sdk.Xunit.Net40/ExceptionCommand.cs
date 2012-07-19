// <copyright file="ExceptionCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Xunit
{
    using System;
    using global::Xunit.Sdk;

    public class ExceptionCommand : global::Xunit.Sdk.TestCommand
    {
        private readonly Exception exception;

        public ExceptionCommand(IMethodInfo method, Exception exception)
            : base(method, null, -1)
        {
            this.exception = exception;
        }

        public override MethodResult Execute(object testClass)
        {
            ExceptionUtility.RethrowWithNoStackTraceLoss(this.exception);
            return null;
        }
    }
}