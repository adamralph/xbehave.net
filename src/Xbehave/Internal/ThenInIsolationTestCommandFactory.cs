// <copyright file="ThenInIsolationTestCommandFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using Xunit.Sdk;

    internal class ThenInIsolationTestCommandFactory
    {
        public IEnumerable<ITestCommand> Create(DisposableStep given, Step when, IEnumerable<Step> thens, string name, IMethodInfo method)
        {
            foreach (var then in thens)
            {
                // do not capture the iteration variable because 
                // all tests would point to the same assertion
                var localThen = then;
                Action test = () =>
                {
                    using (given != null ? given.Execute() : null)
                    {
                        if (when != null)
                        {
                            when.Execute();
                        }

                        localThen.Execute();
                    }
                };

                var testName = string.Format("{0}, {1}", name, then.Message);
                yield return new ActionTestCommand(method, testName, MethodUtility.GetTimeoutParameter(method), test);
            }
        }
    }
}