using System;
using System.Diagnostics.CodeAnalysis;

namespace Xbehave.Sdk
{
    /// <summary>
    /// A scenario step context.
    /// </summary>
    /// <remarks>This is the type provided as an argument to overloads of <c>string.x()</c> and <c>string.Teardown()</c>.</remarks>
    public interface IStepContext
    {
        /// <summary>
        /// Gets the step which owns this context.
        /// </summary>
        IStep Step { get; }

        /// <summary>
        /// Immediately registers the <see cref="IDisposable"/> object for disposal
        /// after all steps in the current scenario have been executed.
        /// </summary>
        /// <param name="disposable">The object to be disposed.</param>
        /// <returns>The current <see cref="IStepContext"/>.</returns>
        IStepContext Using(IDisposable disposable);
    }
}
