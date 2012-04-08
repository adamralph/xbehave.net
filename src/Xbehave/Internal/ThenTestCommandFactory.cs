// <copyright file="ThenTestCommandFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    internal class ThenTestCommandFactory
    {
        public IEnumerable<ITestCommand> Create(DisposableStep given, Step when, IEnumerable<Step> thens, string name, IMethodInfo method)
        {
            if (!thens.Any())
            {
                yield break;
            }

            var givenOrWhenThrewException = false;
            var arrangement = default(IDisposable);

            Action setupAction = () =>
            {
                try
                {
                    if (given != null)
                    {
                        arrangement = given.Execute();
                    }

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

            foreach (var then in thens)
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