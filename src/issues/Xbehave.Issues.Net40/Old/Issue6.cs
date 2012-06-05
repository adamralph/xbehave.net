// <copyright file="Issue6.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues.Old
{
    using System;

    using FluentAssertions;

    using Xbehave;

    /// <summary>
    /// https://bitbucket.org/adamralph/subspecgwt/issue/6/add-missing-given-overload-to-_
    /// </summary>
    public class Issue6
    {
        [Specification]
        public static void AutonamingWithExplicitDisposalAction()
        {
            _.Given(
                () => SomeUglyResourceHungryStaticThirdPartyDependency.CreateResourceConsumingObject(),
                () => SomeUglyResourceHungryStaticThirdPartyDependency.DestroyResourceConsumingObject())
            .When(() => SomeUglyResourceHungryStaticThirdPartyDependency.DoSomethingEarthShatteringlyImportant())
            .Then(() => true.Should().Be(false)).InIsolation();
        }

        private static class SomeUglyResourceHungryStaticThirdPartyDependency
        {
            public static void CreateResourceConsumingObject()
            {
                Console.WriteLine("CREATED");
            }

            public static void DoSomethingEarthShatteringlyImportant()
            {
                Console.WriteLine("USED");
            }

            public static void DestroyResourceConsumingObject()
            {
                Console.WriteLine("DESTROYED");
            }
        }
    }
}
