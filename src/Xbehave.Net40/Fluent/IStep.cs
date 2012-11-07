// <copyright file="IStep.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A scenario step.
    /// </summary>
    public partial interface IStep
    {
        /// <summary>
        /// An optional fluent conjunction.
        /// </summary>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "By design.")]
        IStep And();

        /// <summary>
        /// Indicate that execution of the defined step should be cancelled after a specified timeout.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="System.Threading.Timeout.Infinite"/> (-1) to wait indefinitely.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        IStep WithTimeout(int millisecondsTimeout);

        /// <summary>
        /// Indicates that an isolated context will be created containing this step and a copy of all preceding steps.
        /// </summary>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        IStep InIsolation();

        /// <summary>
        /// Indicates that the step will not be executed.
        /// </summary>
        /// <param name="reason">The reason for not executing the step.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        IStep Skip(string reason);

        /// <summary>
        /// Declares a teardown action (related to this step and/or previous steps) which will be executed after all steps in the current scenario have been executed.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        IStep Teardown(Action action);
    }
}