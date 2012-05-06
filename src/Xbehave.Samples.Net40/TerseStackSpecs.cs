// <copyright file="TerseStackSpecs.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Samples
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Xbehave;

    public class TerseStackSpecs
    {
        [Scenario]
        public void Push()
        {
            var target = default(Stack<int>);
            var element = default(int);

            "Given an element"._(
                () =>
                {
                    element = 11;
                    target = new Stack<int>();
                });

            "when pushing the element"._(
                () => target.Push(element));

            "then the target should not be empty"._(
                () => target.Should().NotBeEmpty());

            "and the target peek should be the element"._(
                () => target.Peek().Should().Be(element));

            "and in isolation the target peek should be the element"._(
                () => target.Peek().Should().Be(element),
                inIsolation: true);

            "and skip the target peek should be the element"._(
                () => target.Peek().Should().Be(element),
                skip: "because I can");
        }
    }
}
