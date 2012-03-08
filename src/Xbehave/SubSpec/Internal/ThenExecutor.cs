// <copyright file="ThenExecutor.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    internal class ThenExecutor
    {
        private readonly Step<ContextDelegate> given;
        private readonly Step<Action> when;
        private readonly IEnumerable<Step<Action>> thens;

        public ThenExecutor(Step<ContextDelegate> given, Step<Action> when, IEnumerable<Step<Action>> thens)
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
                    arrangement = StepExecutor.Execute(given);

                    if (when != null)
                    {
                        StepExecutor.Execute(when);
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
                var capturableObservation = then;
                Action perform = () =>
                {
                    if (givenOrWhenThrewException)
                    {
                        throw new EitherGivenOrWhenFailedException("Execution of Given or When failed.");
                    }

                    StepExecutor.Execute(capturableObservation);
                };

                yield return new ActionTestCommand(method, "\t- " + capturableObservation.Message, 0, perform);
            }

            Action disposal = () =>
            {
                if (arrangement != null)
                {
                    arrangement.Dispose();
                }

                if (givenOrWhenThrewException)
                {
                    throw new EitherGivenOrWhenFailedException("Execution of Given or When failed but arrangement was disposed.");
                }
            };

            yield return new ActionTestCommand(method, "} " + name, 0, disposal);
        }
    }
}