// <copyright file="BackgroundCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using Xunit.Sdk;

    public class BackgroundCommand : FactCommand
    {
        public BackgroundCommand(IMethodInfo method)
            : base(method)
        {
        }

        public override MethodResult Execute(object testClass)
        {
            CurrentScenario.AddingBackgroundSteps = true;
            try
            {
                return base.Execute(testClass);
            }
            finally
            {
                CurrentScenario.AddingBackgroundSteps = false;
            }
        }
    }
}