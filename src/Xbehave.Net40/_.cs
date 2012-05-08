// <copyright file="_.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Fluent;
    using Xbehave.Infra;
    using Xbehave.Internal;

    /// <summary>
    /// Provides access to step definition methods.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "_", Justification = "By design.")]
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "By design.")]
    public static partial class _
    {
        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Given(string message, Func<IDisposable> body)
        {
            return CurrentThread.Enqueue(new Step("Given", message, body, false, null));
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Given(string message, Action body)
        {
            return CurrentThread.Enqueue(new Step("Given", message, body, false, null));
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a collection of resources which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The function that will perform the step and return the disposable resources.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition Given(string message, Func<IEnumerable<IDisposable>> body)
        {
            return CurrentThread.Enqueue(new Step("Given", message, body, false, null));
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <param name="dispose">The action that will dispose the resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Given(string message, Action body, Action dispose)
        {
            return CurrentThread.Enqueue(new Step("Given", message, body, dispose, false, null));
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition When(string message, Func<IDisposable> body)
        {
            return CurrentThread.Enqueue(new Step("When", message, body, false, null));
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition When(string message, Action body)
        {
            return CurrentThread.Enqueue(new Step("When", message, body, false, null));
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a collection of resources which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The function that will perform the step and return the disposable resources.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition When(string message, Func<IEnumerable<IDisposable>> body)
        {
            return CurrentThread.Enqueue(new Step("When", message, body, false, null));
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <param name="dispose">The action that will dispose the resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition When(string message, Action body, Action dispose)
        {
            return CurrentThread.Enqueue(new Step("When", message, body, dispose, false, null));
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Then(string message, Action body)
        {
            return CurrentThread.Enqueue(new Step("Then", message, body, false, null));
        }

        /// <summary>
        /// Defines a step in the current scenario for which an isolated context will be created containing this step and a copy of all preceding steps.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition ThenInIsolation(string message, Action body)
        {
            return CurrentThread.Enqueue(new Step("Then", message, body, true, null));
        }

        /// <summary>
        /// Defines a step in the current scenario that it will not be executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which would have performed the step.</param>
        /// <param name="reason">The reason for not executing the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition ThenSkip(string message, Action body, string reason)
        {
            return CurrentThread.Enqueue(new Step("Then", message, body, false, reason));
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition And(string message, Func<IDisposable> body)
        {
            return CurrentThread.Enqueue(new Step("And", message, body, false, null));
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition And(string message, Action body)
        {
            return CurrentThread.Enqueue(new Step("And", message, body, false, null));
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a collection of resources which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The function that will perform the step and return the disposable resources.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition And(string message, Func<IEnumerable<IDisposable>> body)
        {
            return CurrentThread.Enqueue(new Step("And", message, body, false, null));
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The function that will perform the step and return the disposable resource.</param>
        /// <param name="dispose">The action that will dispose the resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition And(string message, Action body, Action dispose)
        {
            return CurrentThread.Enqueue(new Step("And", message, body, dispose, false, null));
        }

        /// <summary>
        /// Defines a step in the current scenario for which an isolated context will be created containing this step and a copy of all preceding steps.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition AndInIsolation(string message, Action body)
        {
            return CurrentThread.Enqueue(new Step("And", message, body, true, null));
        }

        /// <summary>
        /// Defines a step in the current scenario that it will not be executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which would have performed the step.</param>
        /// <param name="reason">The reason for not executing the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition AndSkip(string message, Action body, string reason)
        {
            return CurrentThread.Enqueue(new Step("And", message, body, false, reason));
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition But(string message, Action body)
        {
            return CurrentThread.Enqueue(new Step("But", message, body, false, null));
        }

        /// <summary>
        /// Defines a step in the current scenario for which an isolated context will be created containing this step and a copy of all preceding steps.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition ButInIsolation(string message, Action body)
        {
            return CurrentThread.Enqueue(new Step("But", message, body, true, null));
        }

        /// <summary>
        /// Defines a step in the current scenario that it will not be executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="body">The action which would have performed the step.</param>
        /// <param name="reason">The reason for not executing the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition ButSkip(string message, Action body, string reason)
        {
            return CurrentThread.Enqueue(new Step("But", message, body, false, reason));
        }
    }
}
