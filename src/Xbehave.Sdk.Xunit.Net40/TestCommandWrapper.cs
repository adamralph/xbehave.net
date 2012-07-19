// <copyright file="TestCommandWrapper.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Xunit
{
    using System.Collections.Generic;

    public class TestCommandWrapper : ITestCommand
    {
        private readonly global::Xunit.Sdk.ITestCommand testCommand;

        public TestCommandWrapper(global::Xunit.Sdk.ITestCommand testCommand)
        {
            this.testCommand = testCommand;
        }

        public global::Xunit.Sdk.ITestCommand TestCommand
        {
            get { return this.testCommand; }
        }

        public MethodResult Execute(object feature)
        {
            return new MethodResultWrapper(this.testCommand.Execute(feature));
        }

        public IEnumerable<object> GetParameters()
        {
            var theoryCommand = this.testCommand as global::Xunit.Extensions.TheoryCommand;
            if (theoryCommand == null)
            {
                return new object[0];
            }

            return theoryCommand.Parameters;
        }
    }
}