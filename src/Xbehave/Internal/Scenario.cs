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

        private readonly List<Step> thens = new List<Step>();
        private readonly List<Step> thensInIsolation = new List<Step>();
        private readonly List<Step> thenSkips = new List<Step>();
        private Step given;
        private Step when;

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
            if (this.given != null)
            {
                throw new InvalidOperationException("The scenario has more than one Given step.");
            }

            this.given = new Step(message, arrange);
            return this.given;
        }

        public IStep When(string message, Func<IDisposable> action)
        {
            if (this.when != null)
            {
                throw new InvalidOperationException("The scenario has more than one When step.");
            }

            this.when = new Step(message, action);
            return this.when;
        }

        public IStep ThenInIsolation(string message, Func<IDisposable> assert)
        {
            var step = new Step(message, assert);
            this.thensInIsolation.Add(step);
            return step;
        }

        public IStep Then(string message, Func<IDisposable> assert)
        {
            var step = new Step(message, assert);
            this.thens.Add(step);
            return step;
        }

        public IStep ThenSkip(string message, Func<IDisposable> assert)
        {
            var step = new Step(message, assert);
            this.thenSkips.Add(step);
            return step;
        }

        public IEnumerable<ITestCommand> GetTestCommands(IMethodInfo method)
        {
            return this.thenInIsolationTestCommandFactory.Create(this.given, this.when, this.thensInIsolation, method)
               .Concat(this.thenTestCommandFactory.Create(this.given, this.when, this.thens, method))
               .Concat(this.thenSkipTestCommandFactory.Create(this.given, this.when, this.thenSkips, method));
        }
    }
}