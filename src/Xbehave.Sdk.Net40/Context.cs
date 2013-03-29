﻿// <copyright file="Context.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    public class Context
    {
        [ThreadStatic]
        private static string failedStepName;

        private readonly MethodCall methodCall;
        private readonly Step[] steps;

        public Context(MethodCall methodCall, IEnumerable<Step> steps)
        {
            Guard.AgainstNullArgument("steps", steps);

            this.methodCall = methodCall;
            this.steps = steps.ToArray();
        }

        public static string FailedStepName
        {
            get { return failedStepName; }
            set { failedStepName = value; }
        }

        public IEnumerable<ITestCommand> CreateCommands(int contextOrdinal)
        {
            FailedStepName = null;
            var stepOrdinal = 1;
            foreach (var step in this.steps)
            {
                yield return new StepCommand(this.methodCall, contextOrdinal, stepOrdinal++, step);
            }

            FailedStepName = null;

            // NOTE: this relies on the test runner executing each above yielded step command and below yielded disposal command as soon as it is recieved
            // TD.NET, R# and xunit.console all seem to do this
            var odd = true;
            while (true)
            {
                var teardowns = CurrentScenario.ExtractTeardowns().ToArray();
                if (!teardowns.Any())
                {
                    break;
                }

                // don't reverse even disposables since their creation order has already been reversed by the previous command
                yield return new TeardownCommand(this.methodCall, contextOrdinal, stepOrdinal++, odd ? teardowns.Reverse() : teardowns);
                odd = !odd;
            }
        }
    }
}