// <copyright file="ThenTestFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Xunit.Sdk;

    internal class ThenTestFactory : ITestFactory
    {
        private readonly ISharedContextTestNameFactory nameFactory;
        private readonly IDisposer disposer;

        public ThenTestFactory(ISharedContextTestNameFactory nameFactory, IDisposer disposer)
        {
            this.nameFactory = nameFactory;
            this.disposer = disposer;
        }

        public IEnumerable<ITestCommand> Create(IEnumerable<Step> contextSteps, IEnumerable<Step> thens, IMethodInfo method)
        {
            if (!thens.Any())
            {
                yield break;
            }

            var disposables = new Stack<IDisposable>();
            Step badContextStep = null;

            Action setup = () =>
            {
                foreach (var step in contextSteps)
                {
                    try
                    {
                        disposables.Push(step.Execute());
                    }
                    catch (Exception)
                    {
                        badContextStep = step;
                        throw;
                    }
                }
            };

            yield return new ActionCommand(method, this.nameFactory.CreateContext(contextSteps), MethodUtility.GetTimeoutParameter(method), setup);

            foreach (var then in thens)
            {
                // take a local copy otherwise all tests would point to the same step
                var localThen = then;
                Action test = () =>
                {
                    ThrowIfBadContextStep(badContextStep);
                    localThen.Execute();
                };

                yield return new ActionCommand(method, this.nameFactory.Create(contextSteps, then), MethodUtility.GetTimeoutParameter(method), test);
            }

            Action disposal = () =>
            {
                this.disposer.Dispose(disposables);
                ThrowIfBadContextStep(badContextStep);
            };

            yield return new ActionCommand(method, this.nameFactory.CreateDisposal(contextSteps), MethodUtility.GetTimeoutParameter(method), disposal);
        }

        private static void ThrowIfBadContextStep(Step throwingStep)
        {
            if (throwingStep != null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Execution of \"{0}\" failed.", throwingStep.Message));
            }
        }
    }
}