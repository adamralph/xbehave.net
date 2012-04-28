// <copyright file="ThenInIsolationTestFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    internal class ThenInIsolationTestFactory : ITestFactory
    {
        private readonly ITestNameFactory nameFactory;
        private readonly IDisposer disposer;

        public ThenInIsolationTestFactory(ITestNameFactory nameFactory, IDisposer disposer)
        {
            this.nameFactory = nameFactory;
            this.disposer = disposer;
        }

        public IEnumerable<ITestCommand> Create(IEnumerable<Step> contextSteps, IEnumerable<Step> thens, IMethodInfo method)
        {
            foreach (var then in thens)
            {
                // take a local copy otherwise all tests would point to the same step
                var localThen = then;
                Action test = () =>
                {
                    var disposables = new Stack<IDisposable>();
                    try
                    {
                        foreach (var step in contextSteps)
                        {
                            disposables.Push(step.Execute());
                        }

                        localThen.Execute();
                    }
                    finally
                    {
                        this.disposer.Dispose(disposables);
                    }
                };

                yield return new ActionCommand(
                    method, this.nameFactory.Create(contextSteps.Concat(then.AsEnumerable())), MethodUtility.GetTimeoutParameter(method), test);
            }
        }
    }
}