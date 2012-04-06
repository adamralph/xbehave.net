// <copyright file="ThenExecutor.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    internal class ThenExecutor
    {
        private readonly DisposableStep given;
        private readonly Step when;
        private readonly IEnumerable<Step> thens;

        public ThenExecutor(DisposableStep given, Step when, IEnumerable<Step> thens)
        {
            this.thens = thens;
            this.given = given;
            this.when = when;
        }

        public IEnumerable<ITestCommand> Commands(string name, IMethodInfo method)
        {
            if (!this.thens.Any())
            {
                yield break;
            }

            var givenOrWhenThrewException = false;
            var arrangement = default(IDisposable);

            Action setupAction = () =>
            {
                try
                {
                    arrangement = given.Execute();

                    if (when != null)
                    {
                        when.Execute();
                    }
                }
                catch (Exception)
                {
                    givenOrWhenThrewException = true;
                    throw;
                }
            };

            yield return new ActionTestCommand(method, "{ " + name, 0, setupAction);

            foreach (var then in this.thens)
            {
                // do not capture the iteration variable because 
                // all tests would point to the same observation
                var localThen = then;
                Action perform = () =>
                {
                    if (givenOrWhenThrewException)
                    {
                        throw new InvalidOperationException("Execution of Given or When failed.");
                    }

                    localThen.Execute();
                };

                yield return new ActionTestCommand(method, "\t- " + localThen.Message, 0, perform);
            }

            Action disposal = () =>
            {
                if (arrangement != null)
                {
                    arrangement.Dispose();
                }

                if (givenOrWhenThrewException)
                {
                    throw new InvalidOperationException("Execution of Given or When failed but arrangement was disposed.");
                }
            };

            yield return new ActionTestCommand(method, "} " + name, 0, disposal);
        }
    }
}