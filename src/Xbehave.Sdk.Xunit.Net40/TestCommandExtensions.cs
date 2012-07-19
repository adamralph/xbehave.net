// <copyright file="TestCommandExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Xunit
{
    using global::Xunit.Extensions;
    using global::Xunit.Sdk;

    public static class TestCommandExtensions
    {
        public static object[] GetParameters(this ITestCommand command)
        {
            var theoryCommand = command as TheoryCommand;
            if (theoryCommand == null)
            {
                return new object[0];
            }

            return theoryCommand.Parameters;
        }
    }
}
