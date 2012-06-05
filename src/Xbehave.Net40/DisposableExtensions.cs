// <copyright file="DisposableExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using Xbehave.Infra;
    using Xbehave.Sdk;

    /// <summary>
    /// Provides methods for adding disposal of objects after scenario execution.
    /// </summary>
    public static class DisposableExtensions
    {
        /// <summary>
        /// Adds disposal of the <see cref="IDisposable"/> after scenario execution.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to be disposed.</param>
        /// <returns>The object.</returns>
        public static T WithDisposal<T>(this T obj) where T : IDisposable
        {
            CurrentScenario.AddDisposable(obj);
            return obj;
        }

        /// <summary>
        /// Adds teardown related to the object after scenario execution.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object related to the teardown.</param>
        /// <param name="teardown">The action which performs the teardown.</param>
        /// <returns>The object.</returns>
        public static T WithTeardown<T>(this T obj, Action teardown)
        {
            CurrentScenario.AddDisposable(new Disposable(teardown));
            return obj;
        }
    }
}
