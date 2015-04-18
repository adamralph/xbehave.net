// <copyright file="IStepContext.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A scenario step context.
    /// </summary>
    public interface IStepContext
    {
        /// <summary>
        /// Immediately registers the <see cref="IDisposable"/> object for disposal
        /// after all steps in the current scenario have been executed.
        /// </summary>
        /// <param name="disposable">The object to be disposed.</param>
        /// <returns>The current <see cref="IStepContext"/>.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Using", Justification = "Makes sense here.")]
        IStepContext Using(IDisposable disposable);
    }
}
