// <copyright file="ActionExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;

    internal static class ActionExtensions
    {
        public static Func<TResult> ToDefaultFunc<TResult>(this Action action)
        {
            return () =>
            {
                action();
                return default(TResult);
            };
        }
    }
}
