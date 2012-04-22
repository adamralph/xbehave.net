// <copyright file="IStep.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    /// <summary>
    /// A scenario step.
    /// </summary>
    public interface IStep
    {
        /// <summary>
        /// Indicate that execution of this step should be cancelled after a specified timeout.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="System.Threading.Timeout.Infinite"/> (-1) to wait indefinitely.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        IStep WithTimeout(int millisecondsTimeout);
    }
}