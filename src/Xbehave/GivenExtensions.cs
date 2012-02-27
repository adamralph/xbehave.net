// <copyright file="GivenExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides extensions for a fluent specification syntax with auto-generated specification names.
    /// </summary>
    public static class GivenExtensions
    {
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <param name="act">The action that will perform the act.</param>
        /// <returns>An instance of <see cref="IWhen"/>.</returns>
        public static IWhen When(this IGiven spec, Expression<Action> act)
        {
            return _.When(act);
        }
    }
}
