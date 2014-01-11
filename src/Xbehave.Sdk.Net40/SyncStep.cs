using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Xbehave.Sdk
{
    public class SyncStep : Step
    {
        public SyncStep(string name, Action body, object stepType)
            : base(name, body.Method, body.Target, stepType)
        {
        }

        public override void Execute()
        {
            try
            {
                ManualResetEvent @event = new ManualResetEvent(false);

                if (this.MillisecondsTimeout > 0)
                {
                    ThreadPool.QueueUserWorkItem(o => 
                    {
                        this.ExecuteMethodInfo();
                        @event.Set();
                    });

                    // NOTE: we do not call the WaitOne(int) overload because it wasn't introduced until .NET 3.5 SP1 and we want to support pre-SP1
                    if (!@event.WaitOne(this.MillisecondsTimeout, false))
                    {
                        throw new Xunit.Sdk.TimeoutException(this.MillisecondsTimeout);
                    }
                }
                else
                {
                    this.ExecuteMethodInfo();
                }
            }
            finally
            {
                this.teardowns.ForEach(CurrentScenario.AddTeardown);
            }
        }
    }
}
