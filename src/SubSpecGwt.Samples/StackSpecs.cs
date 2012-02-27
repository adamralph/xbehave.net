// <copyright file="StackSpecs.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace SubSpec.Samples
{
    using System.Collections.Generic;

    using FluentAssertions;

    using SubSpec;

    public class StackSpecs
    {
        [Specification]
        public void Push()
        {
            var target = default(Stack<int>);
            var element = default(int);

            "Given an element"
                .Given(() =>
                {
                    element = 11;
                    target = new Stack<int>();
                });

            "when pushing the element"
                .When(() => target.Push(element));

            "then the target should not be empty"
                .Then(() => target.Should().NotBeEmpty());

            "then the target peek should be the element"
                .Then(() => target.Peek().Should().Be(element));
        }
    }
}
