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

        private readonly Queue<Step> givens = new Queue<Step>();
        private readonly Queue<Step> whens = new Queue<Step>();
        private readonly Queue<Step> thens = new Queue<Step>();
        private readonly Queue<Step> thensInIsolation = new Queue<Step>();
        private readonly Queue<Step> thenSkips = new Queue<Step>();
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
            return Enqueue(this.whens, new Step(message, arrange));
        }

        public Step When(string message, Func<IDisposable> action)
        {
            return Enqueue(this.whens, new Step(message, action));
        }

        public Step ThenInIsolation(string message, Func<IDisposable> assert)
        {
            return Enqueue(this.thensInIsolation, new Step(message, assert));
        }

        public Step Then(string message, Func<IDisposable> assert)
        {
            return Enqueue(this.thens, new Step(message, assert));
        }

        public Step ThenSkip(string message, Func<IDisposable> assert)
        {
            return Enqueue(this.thenSkips, new Step(message, assert));
        }

        public IEnumerable<ITestCommand> GetTestCommands(IMethodInfo method)
        {
            var contextSteps = this.givens.Concat(this.whens).ToArray();
            return this.thenInIsolationTestFactory.Create(contextSteps, this.thensInIsolation, method)
               .Concat(this.thenTestFactory.Create(contextSteps, this.thens, method))
               .Concat(this.thenSkipTestFactory.Create(contextSteps, this.thenSkips, method));
        }

        private static Step Enqueue(Queue<Step> list, Step step)
        {
            list.Enqueue(step);
            return step;
        }
    }
}