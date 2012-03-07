// <copyright file="IScenarioPrimitive.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    /// <summary>
    /// A scenario primitive.
    /// </summary>
    public interface IScenarioPrimitive
    {
        /// <summary>
        /// Indicate that execution of this delegate should be canceled after a specified timeout.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="System.Threading.Timeout.Infinite"/> (-1) to wait indefinitely.</param>
        /// <returns>An instance of <see cref="IScenarioPrimitive"/>.</returns>
        IScenarioPrimitive WithTimeout(int millisecondsTimeout);
    }
}