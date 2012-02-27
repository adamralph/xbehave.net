// <copyright file="Issue2.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    using FluentAssertions;

    using Xbehave;

    /// <summary>
    /// https://bitbucket.org/adamralph/subspecgwt/issue/2/auto-generated-test-names
    /// </summary>
    public static class Issue2
    {
        // 'Given the object repository is filled, when clearing, then the count should be 0.'
        [Specification]
        public static void ManualGivenManualWhenAutoThen()
        {
            "Given the object repository is filled,"
                .Given(() => ObjectRepository.Add(1));

            "when clearing"
                .When(ObjectRepository.Clear);

            _.ThenInIsolation(() => ObjectRepository.Count.Should().Be(0));
        }

        // 'Given the object repository is filled, when clear, then the count should be 0.'
        [Specification]
        public static void ManualGivenAutoWhenAutoThen()
        {
            "Given the object repository is filled,"
                .Given(() => ObjectRepository.Add(1));

            _.When(() => ObjectRepository.Clear());
            _.ThenInIsolation(() => ObjectRepository.Count.Should().Be(0));
        }

        // 'Given fill, when clear, then the count should be 0.'
        [Specification]
        public static void AllAuto()
        {
            _.Given(() => ObjectRepository.Add(1));
            _.When(() => ObjectRepository.Clear());
            _.ThenInIsolation(() => ObjectRepository.Count.Should().Be(0));
        }

        // 'Given fill, when clear, then the count should be 0.'
        [Specification]
        public static void AllAutoChained()
        {
            _
            .Given(() => ObjectRepository.Add(1))
            .When(() => ObjectRepository.Clear())
            .ThenInIsolation(() => ObjectRepository.Count.Should().Be(0))
            .Then(() => ObjectRepository.Count.Should().Be(0))
            .ThenSkip(() => ObjectRepository.Count.Should().Be(0))
            .Then(() => ObjectRepository.Count.Should().Be(0))
            .ThenInIsolation(() => ObjectRepository.Count.Should().Be(0));
        }
    }
}
