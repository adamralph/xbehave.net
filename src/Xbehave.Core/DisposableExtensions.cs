using System;
using Xbehave.Sdk;

namespace Xbehave
{
    /// <summary>
    /// <see cref="IDisposable"/> extensions.
    /// </summary>
    public static class DisposableExtensions
    {
        /// <summary>
        /// Immediately registers the <see cref="IDisposable"/> object for disposal
        /// after all steps in the current scenario have been executed.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="disposable">The object to be disposed.</param>
        /// <param name="stepContext">The execution context for the current step.</param>
        /// <returns>The object.</returns>
        public static T Using<T>(this T disposable, IStepContext stepContext)
            where T : IDisposable
        {
            Guard.AgainstNullArgument(nameof(stepContext), stepContext);

            stepContext.Using(disposable);
            return disposable;
        }
    }
}
