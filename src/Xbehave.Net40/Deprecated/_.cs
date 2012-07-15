// <copyright file="_.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using Xbehave.Fluent;

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
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [Obsolete("Use Given().Teardown() instead.")]
        public static IStep Given(string text, Action body, Action dispose)
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
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [Obsolete("Use When().Teardown() instead.")]
        public static IStep When(string text, Action body, Action dispose)
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
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [Obsolete("Use Then().Teardown() instead.")]
        public static IStep Then(string text, Action body, Action dispose)
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
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [Obsolete("Use And().Teardown() instead.")]
        public static IStep And(string text, Action body, Action dispose)
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
        /// An instance of <see cref="IStep"/>.
        /// </returns>
        [Obsolete("Use But().Teardown() instead.")]
        public static IStep But(string text, Action body, Action dispose)
        {
            return Helper.AddStep("But", text, body).Teardown(dispose);
        }
    }
}
