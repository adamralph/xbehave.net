// <copyright file="Issue13.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues
{
    using System;
    using FluentAssertions;
    using Xbehave;

    /// <summary>
    /// https://bitbucket.org/adamralph/xbehave.net/issue/13/add-equivalent-of-gherkin-background
    /// </summary>
    public static class Issue13
    {
        private static ImplicitDisposable firstDisposable;

        [Background]
        public static void Background()
        {
            "Given a first disposable"
                .Given(() => firstDisposable = new ImplicitDisposable().Using());
        }

        [Scenario]
        public static void FirstStepFails()
        {
            var secondDisposable = default(ImplicitDisposable);

            "Given a second disposable"
                .Given(() => secondDisposable = new ImplicitDisposable().Using());

            "When using the first disposable"
                .When(() => firstDisposable.Use());

            "And using the second disposable"
                .When(() => secondDisposable.Use());

            "Then true should be false in isolation"
                .Then(() => true.Should().Be(false)).InIsolation();

            "And true should be false"
                .Then(() => true.Should().Be(false));
        }
    }
}
