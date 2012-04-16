// <copyright file="TestCommandNameFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Linq;

    internal class TestCommandNameFactory : ITestCommandNameFactory
    {
        public string CreateSharedContext(Step given, Step when)
        {
            return string.Concat(Create(given, when), " { (shared context)");
        }

        public string CreateSharedStep(Step given, Step when, Step then)
        {
            return string.Concat(Create(given, when), " | ", Create(then));
        }

        public string CreateDisposal(Step given, Step when)
        {
            return string.Concat(Create(given, when), " } (disposal)");
        }

        public string CreateIsolatedStep(Step given, Step when, Step then)
        {
            return string.Concat(Create(given, when), ", ", Create(then));
        }

        public string CreateSkippedStep(Step given, Step when, Step then)
        {
            return this.CreateIsolatedStep(given, when, then);
        }

        private static string Create(params Step[] steps)
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
