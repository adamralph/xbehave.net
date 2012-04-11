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
            var messages = new[]
            {
                (given == null ? null : given.Message),
                (when == null ? null : when.Message),
                (then == null ? null : then.Message),
            };

            return string.Join(" ", messages.Where(message => message != null).ToArray());
        }

        public string CreateSetup(Step given, Step when)
        {
            var messages = new[]
            {
                (given == null ? null : given.Message),
                (when == null ? null : when.Message)
            };

            return string.Concat(string.Join(" ", messages.Where(message => message != null).ToArray()), " (setup)");
        }

        public string CreateTeardown(Step given, Step when)
        {
            var messages = new[]
            {
                (given == null ? null : given.Message),
                (when == null ? null : when.Message)
            };

            return string.Concat(string.Join(" ", messages.Where(message => message != null).ToArray()), " (teardown)");
        }
    }
}
