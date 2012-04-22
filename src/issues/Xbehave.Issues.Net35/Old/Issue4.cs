// <copyright file="Issue4.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues.Old
{
    using FluentAssertions;

    using Xbehave;

    /// <summary>
    /// https://bitbucket.org/adamralph/subspecgwt/issue/4/add-scenario-attributes
    /// </summary>
    public class Issue4
    {
        [Scenario]
        public static void ClearingTheRepositoryContainingOne()
        {
            _.Given(() => ObjectRepository.Add(1))
            .When(() => ObjectRepository.Clear())
            .ThenInIsolation(() => ObjectRepository.Count.Should().Be(0));
        }

        [Scenario]
        [ScenarioData(1)]
        [ScenarioData(2)]
        public static void ClearingTheRepositoryContainingSomeInteger(int integer)
        {
            _.Given(() => ObjectRepository.Add(integer))
            .When(() => ObjectRepository.Clear())
            .ThenInIsolation(() => ObjectRepository.Count.Should().Be(0));
        }

        [Scenario]
        [ScenarioData(1, 2)]
        [ScenarioData(2, 3)]
        public static void ClearingTheRepositoryContainingManyIntegers(int integer1, int integer2)
        {
            "Given the integers are added to the repository,"
                .Given(() =>
                {
                    ObjectRepository.Add(integer1);
                    ObjectRepository.Add(integer2);
                })
                .When(() => ObjectRepository.Clear())
                .ThenInIsolation(() => ObjectRepository.Count.Should().Be(0));
        }

        [Scenario]
        [ScenarioData(1, "a")]
        [ScenarioData('f', null)]
        public static void ClearingTheRepositoryContainingManyObjects<T1, T2>(T1 t1, T2 t2)
        {
            "Given the objects are added to the repository,"
                .Given(() =>
                {
                    ObjectRepository.Add(t1);
                    ObjectRepository.Add(t2);
                })
                .When(() => ObjectRepository.Clear())
                .ThenInIsolation(() => ObjectRepository.Count.Should().Be(0));
        }
    }
}
