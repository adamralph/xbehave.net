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
        private readonly TestCommandNameFactory nameFactory;

        public ThenTestCommandFactory(TestCommandNameFactory nameFactory)
        {
            this.nameFactory = nameFactory;
        }

        public IEnumerable<ITestCommand> Create(DisposableStep given, Step when, IEnumerable<Step> thens, IMethodInfo method)
        {
            if (!thens.Any())
            {
                yield break;
            }

            var arrangement = default(IDisposable);
            var givenThrew = false;
            var whenThrew = false;

            Action setup = () =>
            {
                if (given != null)
                {
                    try
                    {
                        arrangement = given.Execute();
                    }
                    catch (Exception)
                    {
                        givenThrew = true;
                        throw;
                    }
                }

                if (when != null)
                {
                    try
                    {
                        when.Execute();
                    }
                    catch (Exception)
                    {
                        whenThrew = true;
                        throw;
                    }
                }
            };

            yield return new ActionTestCommand(method, this.nameFactory.CreateSetup(given, when), MethodUtility.GetTimeoutParameter(method), setup);

            foreach (var then in thens)
            {
                // take a local copy otherwise all tests would point to the same step
                var localThen = then;
                Action test = () =>
                {
                    ThrowIfGivenOrWhenFailed(givenThrew, whenThrew);
                    localThen.Execute();
                };

                yield return new ActionTestCommand(method, this.nameFactory.Create(given, when, then), MethodUtility.GetTimeoutParameter(method), test);
            }

            Action teardown = () =>
            {
                if (arrangement != null)
                {
                    arrangement.Dispose();
                }

                ThrowIfGivenOrWhenFailed(givenThrew, whenThrew);
            };

            yield return new ActionTestCommand(method, this.nameFactory.CreateTeardown(given, when), MethodUtility.GetTimeoutParameter(method), teardown);
        }

        private static void ThrowIfGivenOrWhenFailed(bool givenThrew, bool whenThrew)
        {
            if (givenThrew)
            {
                throw new InvalidOperationException("Execution of Given failed.");
            }

            if (whenThrew)
            {
                throw new InvalidOperationException("Execution of When failed.");
            }
        }
    }
}