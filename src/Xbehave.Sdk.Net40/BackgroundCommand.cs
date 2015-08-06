// <copyright file="BackgroundCommand.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using Xunit.Sdk;

    public class BackgroundCommand : FactCommand
    {
        public BackgroundCommand(IMethodInfo method)
            : base(method)
        {
        }

        public override MethodResult Execute(object testClass)
        {
            // NOTE: can unify the responsibility for background step processing in CurrentScenario and remove this type
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
