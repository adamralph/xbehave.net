// <copyright file="Issue1.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues.Old
{
    using FluentAssertions;

    using Xbehave;

    /// <summary>
    /// https://bitbucket.org/adamralph/subspecgwt/issue/1/the-test-name-does-not-render-correctly
    /// </summary>
    public class Issue1
    {
        [Specification]
        public void UsingContextDoObservation()
        {
            var name = default(string);

            "Given a name with whitespace"
                .Context(() => name = "Adam ");

            "when trimming"
                .Do(() => name.Trim());

            "the name has no whitespace"
                .Observation(() => true.Should().Be(false));
        }

        [Specification]
        public void UsingGivenWhenThen()
        {
            var name = default(string);

            "Given a name with whitespace"
                .Given(() => name = "Adam ");

            "when trimming"
                .When(() => name.Trim());

            "the name has no whitespace"
                .Then(() => true.Should().Be(false));
        }

        [Specification]
        public void UsingContextDoAssert()
        {
            var name = default(string);

            "Given a name with whitespace"
                .Context(() => name = "Adam ");

            "when trimming"
                .Do(() => name.Trim());

            "the name has no whitespace"
                .Assert(() => true.Should().Be(false));
        }

        [Specification]
        public void UsingGivenWhenThenInIsolation()
        {
            var name = default(string);

            "Given a name with whitespace"
                .Given(() => name = "Adam ");

            "when trimming"
                .When(() => name.Trim());

            "the name has no whitespace"
                .ThenInIsolation(() => true.Should().Be(false));
        }
    }
}
