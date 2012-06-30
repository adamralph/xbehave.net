// <copyright file="_.cs" company="Adam Ralph">
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
    public static partial class _
    {
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
        public static IStepDefinition Given(string text, Action body, Action dispose)
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
        public static IStepDefinition When(string text, Action body, Action dispose)
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
        public static IStepDefinition Then(string text, Action body, Action dispose)
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
        public static IStepDefinition And(string text, Action body, Action dispose)
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
        public static IStepDefinition But(string text, Action body, Action dispose)
        {
            return Helper.AddStep("But", text, body).Teardown(dispose);
        }
    }
}
