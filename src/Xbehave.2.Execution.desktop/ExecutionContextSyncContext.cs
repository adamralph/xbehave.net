namespace Xbehave.Execution
{
    using System.Threading;

    class ExecutionContextSyncContext : SynchronizationContext
    {
        private ExecutionContext executionContext;
        private SynchronizationContext innerContext;

        public ExecutionContextSyncContext(ExecutionContext executionContext, SynchronizationContext innerContext)
        {
            this.executionContext = executionContext;
            this.innerContext = innerContext;
        }

        public ExecutionContext ExecutionContext
        {
            get { return this.executionContext; }
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            this.innerContext.Post(_ => this.Abracadabra(d, _), null);
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            this.innerContext.Send(_ => this.Abracadabra(d, _), state);
        }

        private void Abracadabra(SendOrPostCallback d, object state)
        {
            ExecutionContext.Run(this.executionContext, _ =>
            {
                try
                {
                    d(state);
                }
                finally
                {
                    this.executionContext = ExecutionContext.Capture();
                }
            }, null);
        }
    }
}
