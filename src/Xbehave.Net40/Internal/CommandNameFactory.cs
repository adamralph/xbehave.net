// <copyright file="CommandNameFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using System.Linq;

    internal class CommandNameFactory : ICommandNameFactory
    {
        public string Create(IEnumerable<Step> steps)
        {
            var messages = steps
                .Where(step => step != null)
                .Select(step => step.Message)
                .Where(message => !string.IsNullOrEmpty(message))
                .Select(message => new string(message.SkipWhile(x => !IsValid(x)).TakeWhile(x => IsValid(x)).ToArray()))
                .Where(message => message.Length > 0);

            return string.Join(", ", messages.ToArray());
        }

        private static bool IsValid(char x)
        {
            return !(char.IsWhiteSpace(x) || x == ',');
        }
    }
}
