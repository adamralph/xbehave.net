// <copyright file="ISharedContextCommandNameFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;

    internal interface ISharedContextCommandNameFactory
    {
        string CreateContext(IEnumerable<Step> steps);

        string Create(IEnumerable<Step> contextSteps, Step then);

        string CreateDisposal(IEnumerable<Step> steps);
    }
}
