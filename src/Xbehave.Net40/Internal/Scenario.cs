// <copyright file="Scenario.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
#if NET40
    using System.Dynamic;
#endif
    using System.Linq;
    using Xunit.Sdk;

    internal class Scenario
    {
        private readonly ICommandFactory thenInIsolationTestFactory;
        private readonly ICommandFactory thenTestFactory;
        private readonly ICommandFactory thenSkipTestFactory;

        private readonly List<Step> givens = new List<Step>();
        private readonly List<Step> whens = new List<Step>();
        private readonly List<Step> thens = new List<Step>();
        private readonly List<Step> thensInIsolation = new List<Step>();
        private readonly List<Step> thenSkips = new List<Step>();
#if NET40
        private readonly dynamic context = new ExpandoObject();
#endif

        public Scenario(
            ICommandFactory thenInIsolationTestFactory,
            ICommandFactory thenTestFactory,
            ICommandFactory thenSkipTestFactory)
        {
            this.thenInIsolationTestFactory = thenInIsolationTestFactory;
            this.thenTestFactory = thenTestFactory;
            this.thenSkipTestFactory = thenSkipTestFactory;
        }

#if NET40
        public dynamic Context
        {
            get { return this.context; }
        }

#endif
        public Step Given(string message, Func<IDisposable> arrange)
        {
            return AddStep(this.givens, message, arrange);
        }

        public Step When(string message, Func<IDisposable> action)
        {
            return AddStep(this.whens, message, action);
        }

        public Step ThenInIsolation(string message, Func<IDisposable> assert)
        {
            return AddStep(this.thensInIsolation, message, assert);
        }

        public Step Then(string message, Func<IDisposable> assert)
        {
            return AddStep(this.thens, message, assert);
        }

        public Step ThenSkip(string message, Func<IDisposable> assert)
        {
            return AddStep(this.thenSkips, message, assert);
        }

        public IEnumerable<ITestCommand> GetTestCommands(IMethodInfo method)
        {
            var contextSteps = this.givens.Concat(this.whens).ToArray();
            return this.thenInIsolationTestFactory.Create(contextSteps, this.thensInIsolation, method)
               .Concat(this.thenTestFactory.Create(contextSteps, this.thens, method))
               .Concat(this.thenSkipTestFactory.Create(contextSteps, this.thenSkips, method));
        }

        private static Step AddStep(IList<Step> list, string message, Func<IDisposable> func)
        {
            var step = new Step(message, func);
            list.Add(step);
            return step;
        }
    }
}