// <copyright file="TestCommandFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    internal static class TestCommandFactory
    {
        public static IEnumerable<ITestCommand> Create(
            DisposableStep given, Step when, IEnumerable<Step> thens, IEnumerable<Step> thensInIsolation, IEnumerable<Step> thenSkips, IMethodInfo method)
        {
            var messages = new[] { (given == null ? null : given.Message), (when == null ? null : when.Message) }
                .Where(message => message != null).ToArray();

            var name = string.Join(" ", messages);

            var isolationfactory = new ThenInIsolationTestCommandFactory(given, when, thensInIsolation);
            foreach (var command in isolationfactory.Commands(name, method))
            {
                yield return command;
            }

            var factory = new ThenTestCommandFactory(given, when, thens);
            foreach (var command in factory.Commands(name, method))
            {
                yield return command;
            }

            foreach (var command in thenSkips
                .Select(step => new SkipCommand(method, name + ", " + step.Message, "Action is ThenSkip (instead of Then or ThenInIsolation)")))
            {
                yield return command;
            }
        }
    }
}
