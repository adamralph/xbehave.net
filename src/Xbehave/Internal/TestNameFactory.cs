// <copyright file="TestNameFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using System.Linq;

    internal class TestNameFactory : ITestNameFactory
    {
        public string Create(IEnumerable<Step> steps)
        {
            var tokens = steps
                .Where(step => step != null)
                .Select(step => step.Message)
                .Where(token => !string.IsNullOrEmpty(token))
                .Select(token => token.Trim(' ', ','))
                .Where(token => token.Length > 0);

            return string.Join(", ", tokens.ToArray());
        }
    }
}
