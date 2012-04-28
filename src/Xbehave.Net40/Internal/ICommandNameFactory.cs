// <copyright file="ICommandNameFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;

    internal interface ICommandNameFactory
    {
        string Create(IEnumerable<Step> steps);
    }
}
