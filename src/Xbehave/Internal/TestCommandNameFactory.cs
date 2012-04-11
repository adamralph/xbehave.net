// <copyright file="TestCommandNameFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Linq;

    internal class TestCommandNameFactory
    {
        public string Create(Step given, Step when, Step then)
        {
            return Create(new[] { given, when, then });
        }

        public string CreateSetup(Step given, Step when)
        {
            return string.Concat(Create(new[] { given, when }), " (setup)");
        }

        public string CreateTeardown(Step given, Step when)
        {
            return string.Concat(Create(new[] { given, when }), " (teardown)");
        }

        private static string Create(Step[] steps)
        {
            var tokens = steps.Where(step => step != null)
                .Select(step => step.Message)
                .Where(token => !string.IsNullOrEmpty(token))
                .Select(token => token.Trim())
                .Where(token => token.Length > 0)
                .Select(token => token.Trim(','))
                .Where(token => token.Length > 0);

            return string.Join(", ", tokens.ToArray());
        }
    }
}
