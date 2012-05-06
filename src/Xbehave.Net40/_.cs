// <copyright file="_.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Fluent;
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
        /// <param name="step">The function that will perform the step and return the disposable resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Given(this string message, Func<IDisposable> step)
        {
            return message.ToStepMessage("Given")._(step, false, null);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Given(this string message, Action step)
        {
            return message.ToStepMessage("Given")._(step, false, null);
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a collection of resources which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The function that will perform the step and return the disposable resources.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Given(this string message, Func<IEnumerable<IDisposable>> step)
        {
            return message.ToStepMessage("Given")._(step, false, null);
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The function that will perform the step and return the disposable resource.</param>
        /// <param name="dispose">The action that will dispose the resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Given(this string message, Action step, Action dispose)
        {
            return message.ToStepMessage("Given")._(step, dispose, false, null);
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The function that will perform the step and return the disposable resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition When(this string message, Func<IDisposable> step)
        {
            return message.ToStepMessage("When")._(step, false, null);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition When(this string message, Action step)
        {
            return message.ToStepMessage("When")._(step, false, null);
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a collection of resources which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The function that will perform the step and return the disposable resources.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition When(this string message, Func<IEnumerable<IDisposable>> step)
        {
            return message.ToStepMessage("When")._(step, false, null);
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The function that will perform the step and return the disposable resource.</param>
        /// <param name="dispose">The action that will dispose the resource.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition When(this string message, Action step, Action dispose)
        {
            return message.ToStepMessage("When")._(step, dispose, false, null);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Then(this string message, Action step)
        {
            return message.ToStepMessage("Then")._(step, false, null);
        }

        /// <summary>
        /// Defines a step in the current scenario for which an isolated context will be created containing this step and a copy of all preceding steps.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The action which performs the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition ThenInIsolation(this string message, Action step)
        {
            return message.ToStepMessage("Then")._(step, true, null);
        }

        /// <summary>
        /// Defines a step in the current scenario that it will not be executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="reason">The reason for not executing the step.</param>
        /// <param name="step">The action which would have performed the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition ThenSkip(this string message, string reason, Action step)
        {
            return message.ToStepMessage("Then")._(step, false, reason);
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The function that will perform the step and return the disposable resource.</param>
        /// <param name="inIsolation">if set to <c>true</c> an isolated context will be created containing this step and a copy of all preceding steps.</param>
        /// <param name="skip">
        /// Marks the step so that it will not be executed and provides the reason.
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition And(this string message, Func<IDisposable> step, bool inIsolation = false, string skip = null)
        {
            return message.ToStepMessage("And")._(step, inIsolation, skip);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The action which performs the step.</param>
        /// <param name="inIsolation">if set to <c>true</c> an isolated context will be created containing this step and a copy of all preceding steps.</param>
        /// <param name="skip">
        /// Marks the step so that it will not be executed and provides the reason.
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition And(this string message, Action step, bool inIsolation = false, string skip = null)
        {
            return message.ToStepMessage("And")._(step, inIsolation, skip);
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a collection of resources which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The function that will perform the step and return the disposable resources.</param>
        /// <param name="inIsolation">if set to <c>true</c> an isolated context will be created containing this step and a copy of all preceding steps.</param>
        /// <param name="skip">
        /// Marks the step so that it will not be executed and provides the reason.
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition And(this string message, Func<IEnumerable<IDisposable>> step, bool inIsolation = false, string skip = null)
        {
            return message.ToStepMessage("And")._(step, inIsolation, skip);
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The function that will perform the step and return the disposable resource.</param>
        /// <param name="dispose">The action that will dispose the resource.</param>
        /// <param name="inIsolation">if set to <c>true</c> an isolated context will be created containing this step and a copy of all preceding steps.</param>
        /// <param name="skip">Marks the step so that it will not be executed and provides the reason.
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition And(this string message, Action step, Action dispose, bool inIsolation = false, string skip = null)
        {
            return message.ToStepMessage("And")._(step, dispose, false, null);
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The function that will perform the step and return the disposable resource.</param>
        /// <param name="inIsolation">if set to <c>true</c> an isolated context will be created containing this step and a copy of all preceding steps.</param>
        /// <param name="skip">
        /// Marks the step so that it will not be executed and provides the reason.
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition But(this string message, Func<IDisposable> step, bool inIsolation = false, string skip = null)
        {
            return message.ToStepMessage("But")._(step, inIsolation, skip);
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The action which performs the step.</param>
        /// <param name="inIsolation">if set to <c>true</c> an isolated context will be created containing this step and a copy of all preceding steps.</param>
        /// <param name="skip">
        /// Marks the step so that it will not be executed and provides the reason.
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition But(this string message, Action step, bool inIsolation = false, string skip = null)
        {
            return message.ToStepMessage("But")._(step, inIsolation, skip);
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a collection of resources which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The function that will perform the step and return the disposable resources.</param>
        /// <param name="inIsolation">if set to <c>true</c> an isolated context will be created containing this step and a copy of all preceding steps.</param>
        /// <param name="skip">
        /// Marks the step so that it will not be executed and provides the reason.
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition But(this string message, Func<IEnumerable<IDisposable>> step, bool inIsolation = false, string skip = null)
        {
            return message.ToStepMessage("But")._(step, inIsolation, skip);
        }

        /// <summary>
        /// Defines a step in the current scenario which returns a resource which will be disposed after all remaining steps have been executed.
        /// </summary>
        /// <param name="message">A message describing the step.</param>
        /// <param name="step">The function that will perform the step and return the disposable resource.</param>
        /// <param name="dispose">The action that will dispose the resource.</param>
        /// <param name="inIsolation">if set to <c>true</c> an isolated context will be created containing this step and a copy of all preceding steps.</param>
        /// <param name="skip">Marks the step so that it will not be executed and provides the reason.
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition But(this string message, Action step, Action dispose, bool inIsolation = false, string skip = null)
        {
            return message.ToStepMessage("But")._(step, dispose, inIsolation, skip);
        }
    }
}
