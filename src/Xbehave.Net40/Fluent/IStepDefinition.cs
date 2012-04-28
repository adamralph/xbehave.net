// <copyright file="IStepDefinition.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    /// <summary>
    /// The definition of a scenario step.
    /// </summary>
    /// <typeparam name="T">The type of the definition.</typeparam>
    public interface IStepDefinition<T>
    {
        /// <summary>
        /// Indicate that execution of the defined step should be cancelled after a specified timeout.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="System.Threading.Timeout.Infinite"/> (-1) to wait indefinitely.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        T WithTimeout(int millisecondsTimeout);
    }
}