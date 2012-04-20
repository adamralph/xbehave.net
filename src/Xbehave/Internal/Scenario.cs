// <copyright file="Scenario.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xbehave.Fluent;
    using Xunit.Sdk;

    internal class Scenario
    {
        private readonly ITestCommandFactory thenInIsolationTestCommandFactory;
        private readonly ITestCommandFactory thenTestCommandFactory;
        private readonly ITestCommandFactory thenSkipTestCommandFactory;

        private readonly List<Step> givens = new List<Step>();
        private readonly List<Step> whens = new List<Step>();
        private readonly List<Step> thens = new List<Step>();
        private readonly List<Step> thensInIsolation = new List<Step>();
        private readonly List<Step> thenSkips = new List<Step>();

        public Scenario(
            ITestCommandFactory thenInIsolationTestCommandFactory,
            ITestCommandFactory thenTestCommandFactory,
            ITestCommandFactory thenSkipTestCommandFactory)
        {
            this.thenInIsolationTestCommandFactory = thenInIsolationTestCommandFactory;
            this.thenTestCommandFactory = thenTestCommandFactory;
            this.thenSkipTestCommandFactory = thenSkipTestCommandFactory;
        }

        public IStep Given(string message, Func<IDisposable> arrange)
        {
            return AddStep(this.givens, message, arrange);
        }

        public IStep When(string message, Func<IDisposable> action)
        {
            return AddStep(this.whens, message, action);
        }

        public IStep ThenInIsolation(string message, Func<IDisposable> assert)
        {
            return AddStep(this.thensInIsolation, message, assert);
        }

        public IStep Then(string message, Func<IDisposable> assert)
        {
            return AddStep(this.thens, message, assert);
        }

        public IStep ThenSkip(string message, Func<IDisposable> assert)
        {
            return AddStep(this.thenSkips, message, assert);
        }

        public IEnumerable<ITestCommand> GetTestCommands(IMethodInfo method)
        {
            return this.thenInIsolationTestCommandFactory.Create(this.givens, this.whens, this.thensInIsolation, method)
               .Concat(this.thenTestCommandFactory.Create(this.givens, this.whens, this.thens, method))
               .Concat(this.thenSkipTestCommandFactory.Create(this.givens, this.whens, this.thenSkips, method));
        }

        private static IStep AddStep(IList<Step> list, string message, Func<IDisposable> func)
        {
            var step = new Step(message, func);
            list.Add(step);
            return step;
        }
    }
}