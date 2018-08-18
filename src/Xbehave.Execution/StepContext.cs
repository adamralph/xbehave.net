namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using Xbehave.Sdk;

    public class StepContext : IStepContext
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        public StepContext(IStep step) => this.Step = step;

        public IStep Step { get; }

        public IReadOnlyList<IDisposable> Disposables => this.disposables;

        public IStepContext Using(IDisposable disposable)
        {
            if (disposable != null)
            {
                this.disposables.Add(disposable);
            }

            return this;
        }
    }
}
