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

        public IEnumerable<ITestCommand> Create(IEnumerable<Step> givens, IEnumerable<Step> whens, IEnumerable<Step> thens, IMethodInfo method)
        {
            if (!thens.Any())
            {
                yield break;
            }

            var disposables = new Stack<IDisposable>();
            Step throwingStep = null;

            Action setup = () =>
            {
                foreach (var step in givens.Concat(whens))
                {
                    try
                    {
                        disposables.Push(step.Execute());
                    }
                    catch (Exception)
                    {
                        throwingStep = step;
                        throw;
                    }
                }
            };

            yield return new ActionTestCommand(method, this.nameFactory.CreateContext(givens, whens), MethodUtility.GetTimeoutParameter(method), setup);

            foreach (var then in thens)
            {
                // take a local copy otherwise all tests would point to the same step
                var localThen = then;
                Action test = () =>
                {
                    ThrowIfContextThrew(throwingStep);
                    localThen.Execute();
                };

                yield return new ActionTestCommand(method, this.nameFactory.Create(givens, whens, then), MethodUtility.GetTimeoutParameter(method), test);
            }

            Action disposal = () =>
            {
                this.disposer.Dispose(disposables);
                ThrowIfContextThrew(throwingStep);
            };

            yield return new ActionTestCommand(method, this.nameFactory.CreateDisposal(givens, whens), MethodUtility.GetTimeoutParameter(method), disposal);
        }

        private static void ThrowIfContextThrew(Step throwingStep)
        {
            if (throwingStep != null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Execution of \"{0}\" failed.", throwingStep.Message));
            }
        }
    }
}