// <copyright file="Step.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xbehave.Infra;

    internal class Step
    {
        private readonly string name;
        private readonly Func<IDisposable> body;

        public Step(string stepType, string text, Func<IDisposable> body, bool inIsolation, string skipReason)
            : this(stepType, text, inIsolation, skipReason)
        {
            Avoid.NullReferenceExceptionForArgument(body, "body");

            this.body = body;
        }

        public Step(string stepType, string text, Action body, bool inIsolation, string skipReason)
            : this(stepType, text, inIsolation, skipReason)
        {
            Avoid.NullReferenceExceptionForArgument(body, "body");

            this.body = new Func<IDisposable>(() =>
            {
                body();
                return default(IDisposable);
            });
        }

        public Step(string stepType, string text, Func<IEnumerable<IDisposable>> body, bool inIsolation, string skipReason)
            : this(stepType, text, inIsolation, skipReason)
        {
            Avoid.NullReferenceExceptionForArgument(body, "body");

            this.body = new Func<IDisposable>(() => new Disposable(body().Reverse()));
        }

        public Step(string stepType, string text, Action body, Action dispose, bool inIsolation, string skipReason)
            : this(stepType, text, inIsolation, skipReason)
        {
            Avoid.NullReferenceExceptionForArgument(body, "body");

            this.body = new Func<IDisposable>(() =>
            {
                body();
                return new Disposable(dispose);
            });
        }

        private Step(string stepType, string text, bool inIsolation, string skipReason)
        {
            Avoid.NullReferenceExceptionForArgument(stepType, "stepType");
            Avoid.NullReferenceExceptionForArgument(text, "text");

            this.name = text.StartingWithOrdinalIgnoreCase(stepType + " ");
            this.InIsolation = inIsolation;
            this.SkipReason = skipReason;
        }

        public string Name
        {
            get { return this.name; }
        }

        public string SkipReason { get; set; }

        public bool InIsolation { get; set; }

        public int MillisecondsTimeout { get; set; }

        public IDisposable Execute()
        {
            if (this.MillisecondsTimeout > 0)
            {
                var result = this.body.BeginInvoke(null, null);

                // NOTE: we do not call the WaitOne(int) overload because it wasn't introduced until .NET 3.5 SP1 and we want to support pre-SP1
                if (!result.AsyncWaitHandle.WaitOne(this.MillisecondsTimeout, false))
                {
                    throw new Xunit.Sdk.TimeoutException(this.MillisecondsTimeout);
                }

                return this.body.EndInvoke(result);
            }

            return this.body();
        }
    }
}