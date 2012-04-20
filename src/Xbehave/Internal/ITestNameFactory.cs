// <copyright file="ITestNameFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;

    internal interface ITestNameFactory
    {
        string CreateSharedContext(IEnumerable<Step> givens, IEnumerable<Step> whens);

        string CreateSharedStep(IEnumerable<Step> givens, IEnumerable<Step> whens, Step then);

        string CreateDisposal(IEnumerable<Step> givens, IEnumerable<Step> whens);

        string CreateIsolatedStep(IEnumerable<Step> givens, IEnumerable<Step> whens, Step then);

        string CreateSkippedStep(IEnumerable<Step> givens, IEnumerable<Step> whens, Step then);
    }
}
