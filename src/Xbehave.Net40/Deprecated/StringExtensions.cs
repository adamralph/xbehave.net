// <copyright file="StringExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Xbehave.Fluent;
    using Xbehave.Infra;

    /// <summary>
    /// Extensions for declaring Given, When, Then scenario steps.
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// Deprecated in version 0.4.0.
        /// </summary>
        /// <param name="text">A text describing the arrangment.</param>
        /// <param name="body">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use Given().Teardown() instead.")]
        public static IStepDefinition GivenDisposable(this string text, ContextDelegate body)
        {
            Guard.AgainstNullArgument("body", body);
            IDisposable disposable = null;
            return text.Given(() => disposable = body(), () => new[] { disposable }.DisposeAll());
        }

        /// <summary>
        /// Deprecated in version 0.10.0.
        /// </summary>
        /// <param name="text">A text describing the assertion.</param>
        /// <param name="body">The action which would have performed the assertion.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use Then().Skip() instead.")]
        public static IStepDefinition ThenSkip(this string text, Action body)
        {
            return text.Then(body).Skip("Skipped for an unknown reason.");
        }

        /// <summary>
        /// Deprecated in version 0.10.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action which performs the step.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use Then().InIsolation() instead.")]
        public static IStepDefinition ThenInIsolation(this string text, Action body)
        {
            return text.Then(body).InIsolation();
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <param name="dispose">An optional action which will perform teardown after execution of the scenario.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use Given().Teardown() instead.")]
        public static IStepDefinition Given(this string text, Action body, Action dispose)
        {
            return Helper.AddStep("Given", text, body).Teardown(dispose);
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <param name="dispose">An optional action which will perform teardown after execution of the scenario.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use When().Teardown() instead.")]
        public static IStepDefinition When(this string text, Action body, Action dispose)
        {
            return Helper.AddStep("When", text, body).Teardown(dispose);
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <param name="dispose">An optional action which will perform teardown after execution of the scenario.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use Then().Teardown() instead.")]
        public static IStepDefinition Then(this string text, Action body, Action dispose)
        {
            return Helper.AddStep("Then", text, body).Teardown(dispose);
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <param name="dispose">An optional action which will perform teardown after execution of the scenario.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use And().Teardown() instead.")]
        public static IStepDefinition And(this string text, Action body, Action dispose)
        {
            return Helper.AddStep("And", text, body).Teardown(dispose);
        }

        /// <summary>
        /// Deprecated in version 0.11.0.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <param name="dispose">An optional action which will perform teardown after execution of the scenario.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [Obsolete("Use But().Teardown() instead.")]
        public static IStepDefinition But(this string text, Action body, Action dispose)
        {
            return Helper.AddStep("But", text, body).Teardown(dispose);
        }
    }
}
