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
        private static readonly ThenInIsolationTestCommandFactory thenInIsolationTestCommandFactory = new ThenInIsolationTestCommandFactory(new TestCommandNameFactory());
        private static readonly ThenTestCommandFactory thenTestCommandFactory = new ThenTestCommandFactory(new TestCommandNameFactory());
        private static readonly ThenSkipTestCommandFactory thenSkipTestCommandFactory = new ThenSkipTestCommandFactory(new TestCommandNameFactory());

        private readonly List<Step> thens = new List<Step>();
        private readonly List<Step> thensInIsolation = new List<Step>();
        private readonly List<Step> thenSkips = new List<Step>();
        private DisposableStep given;
        private Step when;

        public IStep Given(string message, Func<IDisposable> arrange)
        {
            if (this.given != null)
            {
                throw new InvalidOperationException("The scenario has more than one Given step.");
            }

            this.given = new DisposableStep(message, arrange);
            return this.given;
        }

        public IStep When(string message, Action action)
        {
            if (this.when != null)
            {
                throw new InvalidOperationException("The scenario has more than one When step.");
            }

            this.when = new Step(message, action);
            return this.when;
        }

        public IStep ThenInIsolation(string message, Action assert)
        {
            var step = new Step(message, assert);
            this.thensInIsolation.Add(step);
            return step;
        }

        public IStep Then(string message, Action assert)
        {
            var step = new Step(message, assert);
            this.thens.Add(step);
            return step;
        }

        public IStep ThenSkip(string message, Action assert)
        {
            var step = new Step(message, assert);
            this.thenSkips.Add(step);
            return step;
        }

        public IEnumerable<ITestCommand> GetTestCommands(IMethodInfo method)
        {
            return thenInIsolationTestCommandFactory.Create(this.given, this.when, this.thensInIsolation, method)
               .Concat(thenTestCommandFactory.Create(this.given, this.when, this.thens, method))
               .Concat(thenSkipTestCommandFactory.Create(this.given, this.when, this.thenSkips, method));
        }
    }
}