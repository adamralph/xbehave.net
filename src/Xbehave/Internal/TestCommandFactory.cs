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
        private static readonly ThenInIsolationTestCommandFactory thenInIsolationTestCommandFactory = new ThenInIsolationTestCommandFactory(new TestCommandNameFactory());
        private static readonly ThenTestCommandFactory thenTestCommandFactory = new ThenTestCommandFactory(new TestCommandNameFactory());
        private static readonly ThenSkipTestCommandFactory thenSkipTestCommandFactory = new ThenSkipTestCommandFactory(new TestCommandNameFactory());

        public static IEnumerable<ITestCommand> Create(
            DisposableStep given, Step when, IEnumerable<Step> thens, IEnumerable<Step> thensInIsolation, IEnumerable<Step> thenSkips, IMethodInfo method)
        {
            return thenInIsolationTestCommandFactory.Create(given, when, thensInIsolation, method)
                .Concat(thenTestCommandFactory.Create(given, when, thens, method))
                .Concat(thenSkipTestCommandFactory.Create(given, when, thenSkips, method));
        }
    }
}
