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
        private readonly TestCommandNameFactory nameFactory;

        public ThenInIsolationTestCommandFactory(TestCommandNameFactory nameFactory)
        {
            this.nameFactory = nameFactory;
        }

        public IEnumerable<ITestCommand> Create(DisposableStep given, Step when, IEnumerable<Step> thens, IMethodInfo method)
        {
            foreach (var then in thens)
            {
                // take a local copy otherwise all tests would point to the same step
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

                yield return new ActionTestCommand(method, this.nameFactory.Create(given, when, then), MethodUtility.GetTimeoutParameter(method), test);
            }
        }
    }
}