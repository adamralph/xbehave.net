// <copyright file="StringExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
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
        /// <param name="message">A message describing the arrangment.</param>
        /// <param name="arrange">The function that will perform and return the arrangement.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [Obsolete("Use Given(Func<IDisposable>) instead.")]
        public static IStep GivenDisposable(this string message, ContextDelegate arrange)
        {
            Require.NotNull(arrange, "arrange");
            return message.Given(() => arrange());
        }
    }
}
