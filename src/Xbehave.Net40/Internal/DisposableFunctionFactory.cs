// <copyright file="DisposableFunctionFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xbehave.Infra;

    internal static class DisposableFunctionFactory
    {
        public static Func<IDisposable> Create(Func<IDisposable> source)
        {
            return source;
        }

        public static Func<IDisposable> Create(Action source, Action dispose)
        {
            return source == null ? null : new Func<IDisposable>(() =>
                {
                    source();
                    return new Disposable(dispose);
                });
        }

        public static Func<IDisposable> Create(Func<IEnumerable<IDisposable>> source)
        {
            return source == null ? null : new Func<IDisposable>(() => new Disposable(source().Reverse()));
        }

        public static Func<IDisposable> Create(Action source)
        {
            return source == null ? null : new Func<IDisposable>(() =>
                {
                    source();
                    return default(IDisposable);
                });
        }
    }
}