// <copyright file="Issue9.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Xbehave;

    /// <summary>
    /// https://bitbucket.org/adamralph/xbehave.net/issue/9/do-not-throw-exceptions-when-step-name
    /// </summary>
    public class Issue9
    {
        [Scenario]
        public void Push()
        {
            var target = default(Stack<int>);
            var element = default(int);

            "Given an element {0}"
                .Given(() => element = 11);

            "and a stack \"foo\""
                .And(() => target = new Stack<int>());

            "when pushing the element"
                .When(() => target.Push(element));

            "then the target pop should be the element"
                .Then(() => target.Pop().Should().Be(element))
                .InIsolation();

            "then the target should not be empty"
                .Then(() => target.Should().NotBeEmpty());

            "and the target count should be 2"
                .And(() => target.Count.Should().Be(2))
                .Skip("because the target count is 1");
        }
    }
}
