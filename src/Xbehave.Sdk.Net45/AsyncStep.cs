namespace Xbehave.Sdk
{
    using System;
    using System.Threading.Tasks;

    class AsyncStep : Step
    {
        private readonly Func<Task> body;

        public AsyncStep(string name, Func<Task> body, object stepType)
            : base(name, () => { }, stepType)
        {
            this.body = body;
        }

        public override void Execute()
        {
            try
            {
                if (this.MillisecondsTimeout > 0)
                {
                    var bodyTask = this.body();
                    var delay = Task.Delay(this.MillisecondsTimeout);
                    var firstToFinish = Task.WhenAny(delay, bodyTask).Result;

                    if (firstToFinish == delay)
                    {
                        throw new Xunit.Sdk.TimeoutException(this.MillisecondsTimeout);
                    }
                }
                else
                {
                    this.body().Wait();
                }
            }
            finally
            {
                this.teardowns.ForEach(CurrentScenario.AddTeardown);
            }
        }
    }
}
