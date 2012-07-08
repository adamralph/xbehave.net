// <copyright file="Issue14.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues
{
    using System;
    using Xbehave;

    /// <summary>
    /// https://bitbucket.org/adamralph/xbehave.net/issue/14/when-a-step-fails-subsequent-steps-should
    /// </summary>
    public class Issue14
    {
        [Scenario]
        public static void FirstStepFails()
        {
            "Given a thing"
                .Given(() => { throw new Exception(); });

            "When doing something"
                .When(() => { });

            "Then something happens"
                .Then(() => { }).InIsolation();

            "And something else happens"
                .And(() => { });
        }
    }
}
