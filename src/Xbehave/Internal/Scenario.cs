// <copyright file="Scenario.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Xbehave.Fluent;
    using Xunit.Sdk;

    internal class Scenario
    {
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

        // TODO: address DoNotCatchGeneralExceptionTypes
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Part of the original SubSpec code - will be addressed.")]
        public IEnumerable<ITestCommand> GetTestCommands(IMethodInfo method)
        {
            try
            {
                return TestCommandFactory.Create(this.given, this.when, this.thens, this.thensInIsolation, this.thenSkips, method);
            }
            catch (Exception ex)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "An exception was thrown while creating test commands from scenario {0}.{1}:\r\n{2}",
                    method.TypeName,
                    method.Name,
                    ex.Message);

                return new[] { new ExceptionTestCommand(method, () => { throw new InvalidOperationException(message, ex); }) };
            }
        }
    }
}