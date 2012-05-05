// <copyright file="Scenario.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    internal partial class Scenario
    {
        private readonly IAgnosticCommandFactory agnosticCommandFactory;

        private readonly Queue<Step> givens = new Queue<Step>();
        private readonly Queue<Step> whens = new Queue<Step>();
        private readonly Queue<Step> thens = new Queue<Step>();
        private readonly Queue<Step> thensInIsolation = new Queue<Step>();
        private readonly Queue<Step> thenSkips = new Queue<Step>();

        public Scenario(IAgnosticCommandFactory agnosticCommandFactory)
        {
            this.agnosticCommandFactory = agnosticCommandFactory;
        }

        public Step Given(Step step)
        {
            return Enqueue(this.whens, step);
        }

        public Step When(Step step)
        {
            return Enqueue(this.whens, step);
        }

        public Step ThenInIsolation(Step step)
        {
            return Enqueue(this.thensInIsolation, step);
        }

        public Step Then(Step step)
        {
            return Enqueue(this.thens, step);
        }

        public Step ThenSkip(Step step)
        {
            return Enqueue(this.thenSkips, step);
        }

        public IEnumerable<ITestCommand> GetTestCommands(IMethodInfo method)
        {
            return this.agnosticCommandFactory.Create(
                new Queue<Step>(this.givens.Concat(this.whens).Concat(this.thensInIsolation).Concat(this.thens).Concat(this.thenSkips)), method);
        }

        private static Step Enqueue(Queue<Step> list, Step step)
        {
            list.Enqueue(step);
            return step;
        }
    }
}