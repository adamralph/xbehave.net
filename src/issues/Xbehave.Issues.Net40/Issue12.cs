// <copyright file="Issue12.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues
{
    using Xbehave;

    /// <summary>
    /// https://bitbucket.org/adamralph/xbehave.net/issue/12/infinite-loops-in-test-runner-when
    /// </summary>
    public class Issue12
    {
        [Scenario]
        public static void NoDataProvided(int x, int y)
        {
            throw new System.Exception();
        }
    }
}
