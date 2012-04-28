// <copyright file="StepFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xbehave.Infra;

    internal static class StepFactory
    {
        public static Step Create(string message, Func<IDisposable> execute)
        {
            return new Step(message, execute);
        }

        public static Step Create(string message, Action execute, Action dispose)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            Func<IDisposable> func = () =>
            {
                execute();
                return new Disposable(dispose);
            };

            return new Step(message, func);
        }

        public static Step Create(string message, Func<IEnumerable<IDisposable>> execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            return new Step(message, () => new Disposable(execute().Reverse()));
        }

        public static Step Create(string message, Action execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            Func<IDisposable> func = () =>
            {
                execute();
                return default(IDisposable);
            };

            return new Step(message, func);
        }
    }
}