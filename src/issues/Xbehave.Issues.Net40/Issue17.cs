// <copyright file="Issue17.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues
{
    using FluentAssertions;
    using Xbehave;

    /// <summary>
    /// https://bitbucket.org/adamralph/xbehave.net/issue/17
    /// </summary>
    public class Issue17
    {
        protected int Foo { get; set; }

        [Background]
        public virtual void Background()
        {
            "Given foo is 1".Given(() => this.Foo = 1);
        }

        [Scenario]
        public virtual void FooIsGreaterThanZero()
        {
            _.Then(() => this.Foo.Should().BeGreaterThan(0));
        }

        public class FooIsTwo : Issue17
        {
            ////[Background]
            public override void Background()
            {
                "Given foo is 2".Given(() => this.Foo = 2);
            }
        }
    }
}
