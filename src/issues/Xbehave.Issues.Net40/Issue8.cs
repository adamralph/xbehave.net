// <copyright file="Issue8.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues.Old
{
    using Xbehave;

    /// <summary>
    /// https://bitbucket.org/adamralph/xbehave.net/issue/8/fix-infinite-loops-when-running-from-tdnet
    /// </summary>
    public class Issue8
    {
        [Scenario]
        public static void ThrowsException()
        {
            throw new System.Exception();
        }
    }
}
