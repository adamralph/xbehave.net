// <copyright file="DisposableExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using Xbehave.Sdk;

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
        /// <param name="obj">The object to be disposed.</param>
        /// <param name="stepContext">The execution context for the current step.</param>
        /// <returns>The object.</returns>
        public static T Using<T>(this T obj, IStepContext stepContext) where T : IDisposable
        {
            Guard.AgainstNullArgument("stepContext", stepContext);

            stepContext.Using(obj);
            return obj;
        }
    }
}
