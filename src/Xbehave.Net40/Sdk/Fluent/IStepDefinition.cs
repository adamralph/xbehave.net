// <copyright file="IStepDefinition.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The definition of a scenario step.
    /// </summary>
    public partial interface IStepDefinition
    {
        /// <summary>
        /// Indicate that execution of the defined step should be cancelled after a specified timeout.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="System.Threading.Timeout.Infinite"/> (-1) to wait indefinitely.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        IStepDefinition WithTimeout(int millisecondsTimeout);

        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "When", Justification = "By design.")]
        IStepDefinition When(string message, Func<IDisposable> body);

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "When", Justification = "By design.")]
        IStepDefinition When(string message, Action body);

        /// <summary>
        /// Defines a step in the current scenario which returns a collection of resources which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The function that will perform the step and return the disposable resources.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "When", Justification = "By design.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        IStepDefinition When(string message, Func<IEnumerable<IDisposable>> body);

        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <param name="dispose">The action that will dispose the resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "When", Justification = "By design.")]
        IStepDefinition When(string message, Action body, Action dispose);

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Then", Justification = "By design.")]
        IStepDefinition Then(string message, Action body);

        /// <summary>
        /// Defines a step in the current scenario for which an isolated context will be created containing this step and a copy of all preceding steps.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        IStepDefinition ThenInIsolation(string message, Action body);

        /// <summary>
        /// Defines a step in the current scenario that it will not be executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which would have performed the step.</param>
        /// <param name="reason">The reason for not executing the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        IStepDefinition ThenSkip(string message, Action body, string reason);

        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "By design.")]
        IStepDefinition And(string message, Func<IDisposable> body);

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "By design.")]
        IStepDefinition And(string message, Action body);

        /// <summary>
        /// Defines a step in the current scenario which returns a collection of resources which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The function that will perform the step and return the disposable resources.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "By design.")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        IStepDefinition And(string message, Func<IEnumerable<IDisposable>> body);

        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <param name="dispose">The action that will dispose the resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "And", Justification = "By design.")]
        IStepDefinition And(string message, Action body, Action dispose);

        /// <summary>
        /// Defines a step in the current scenario for which an isolated context will be created containing this step and a copy of all preceding steps.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        IStepDefinition AndInIsolation(string message, Action body);

        /// <summary>
        /// Defines a step in the current scenario that it will not be executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which would have performed the step.</param>
        /// <param name="reason">The reason for not executing the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        IStepDefinition AndSkip(string message, Action body, string reason);

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        IStepDefinition But(string message, Action body);

        /// <summary>
        /// Defines a step in the current scenario for which an isolated context will be created containing this step and a copy of all preceding steps.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        IStepDefinition ButInIsolation(string message, Action body);

        /// <summary>
        /// Defines a step in the current scenario that it will not be executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which would have performed the step.</param>
        /// <param name="reason">The reason for not executing the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        IStepDefinition ButSkip(string message, Action body, string reason);
    }
}