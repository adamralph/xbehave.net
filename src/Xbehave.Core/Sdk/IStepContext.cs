// <copyright file="IStepContext.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A scenario step context.
    /// </summary>
    /// <remarks>This is the type provided as an argument to overloads of <code>string.x()</code> and <code>string.Teardown()</code>.</remarks>
    public interface IStepContext
    {
        /// <summary>
        /// Gets the step which owns this context.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Step", Justification = "Makes sense here.")]
        IStep Step { get; }

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
