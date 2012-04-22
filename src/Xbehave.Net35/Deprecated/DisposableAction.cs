// <copyright file="DisposableAction.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// This member is deprecated (was part of the original SubSpec API).
    /// </summary>
    [Obsolete("Use Given(Action arrange, Action dispose) instead.")]
    public class DisposableAction : IDisposable
    {
        /// <summary>
        /// This member is deprecated (was part of the original SubSpec API).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "The reference type is immutable.")]
        public static readonly DisposableAction None = new DisposableAction(() => { });

        private readonly Action action;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableAction"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public DisposableAction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.action = action;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.action();
        }
    }
}