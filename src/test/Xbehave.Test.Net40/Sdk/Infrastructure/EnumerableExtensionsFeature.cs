// <copyright file="EnumerableExtensionsFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Sdk.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Sdk.Infrastructure;

    public static class EnumerableExtensionsFeature
    {
        [Scenario]
        public static void Concatenation()
        {
            var items = default(IEnumerable<int>);
            var newItem = default(int);
            var result = default(IEnumerable<int>);

            "Given some items"
                .Given(() => items = new[] { 1, 2 });

            "And a new item"
                .And(() => newItem = 3);

            "When concatenating the items with the new item"
                .When(() => result = items.Concat(newItem));

            "Then the result should contain the original items followed by the new item"
                .Then(() => result.Should().Equal(items.ElementAt(0), items.ElementAt(1), newItem));
        }
    }
}
