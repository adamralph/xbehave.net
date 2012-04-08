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
        private static readonly ThenInIsolationTestCommandFactory thenInIsolationTestCommandFactory = new ThenInIsolationTestCommandFactory();
        private static readonly ThenTestCommandFactory thenTestCommandFactory = new ThenTestCommandFactory();
        private static readonly ThenSkipTestCommandFactory thenSkipTestCommandFactory = new ThenSkipTestCommandFactory();

        public static IEnumerable<ITestCommand> Create(
            DisposableStep given, Step when, IEnumerable<Step> thens, IEnumerable<Step> thensInIsolation, IEnumerable<Step> thenSkips, IMethodInfo method)
        {
            var messages = new[] { (given == null ? null : given.Message), (when == null ? null : when.Message) }
                .Where(message => message != null).ToArray();

            var name = string.Join(" ", messages);

            return thenInIsolationTestCommandFactory.Create(given, when, thensInIsolation, name, method)
                .Concat(thenTestCommandFactory.Create(given, when, thens, name, method))
                .Concat(thenSkipTestCommandFactory.Create(given, when, thenSkips, name, method));
        }
    }
}
