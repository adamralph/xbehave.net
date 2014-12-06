// <copyright file="IStep.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;

    /// <summary>
    /// A scenario step.
    /// </summary>
    public interface IStep
    {
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