// <copyright file="ThenInIsolationExecutor.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using Xunit.Sdk;

    internal class ThenInIsolationExecutor
    {
        private readonly Step<ContextDelegate> given;
        private readonly Step<Action> when;
        private readonly List<Step<Action>> thens;

        public ThenInIsolationExecutor(Step<ContextDelegate> given, Step<Action> when, List<Step<Action>> thens)
        {
            this.thens = thens;
            this.given = given;
            this.when = when;
        }

        public IEnumerable<ITestCommand> Commands(string name, IMethodInfo method)
        {
            foreach (var then in this.thens)
            {
                // do not capture the iteration variable because 
                // all tests would point to the same assertion
                var capturableAssertion = then;
                Action test = () =>
                {
                    using (StepExecutor.Execute(given))
                    {
                        if (this.when != null)
                        {
                            StepExecutor.Execute(when);
                        }

                        StepExecutor.Execute(capturableAssertion);
                    }
                };

                var testName = string.Format("{0}, {1}", name, then.Message);
                yield return new ActionTestCommand(method, testName, MethodUtility.GetTimeoutParameter(method), test);
            }
        }
    }
}