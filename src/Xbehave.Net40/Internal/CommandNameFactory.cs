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
                .Select(message => message.Trim(' ', ','))
                .Where(message => message.Length > 0);

            return string.Join(", ", messages.ToArray());
        }
    }
}
