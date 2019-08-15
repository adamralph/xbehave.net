namespace Xbehave.Test.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.ExceptionServices;
    using Xunit;
    using Xunit.Abstractions;

    public abstract class Feature : IDisposable
    {
        private readonly IList<Xunit2> runners = new List<Xunit2>();

        ~Feature()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public TMessage[] Run<TMessage>(Assembly assembly, string collectionName)
            where TMessage : IMessageSinkMessage => this.Run(assembly, collectionName).OfType<TMessage>().ToArray();

        public TMessage[] Run<TMessage>(Type feature)
            where TMessage : IMessageSinkMessage => this.Run(feature).OfType<TMessage>().ToArray();

        public TMessage[] Run<TMessage>(Type feature, string traitName, string traitValue)
            where TMessage : IMessageSinkMessage => this.Run(feature, traitName, traitValue).OfType<TMessage>().ToArray();

        public IMessageSinkMessage[] Run(Assembly assembly, string collectionName)
        {
            var runner = this.CreateRunner(assembly.GetLocalCodeBase());
            return runner.Run(runner.Find(collectionName)).ToArray();
        }

        public IMessageSinkMessage[] Run(Type feature)
        {
            var runner = this.CreateRunner(feature.GetTypeInfo().Assembly.GetLocalCodeBase());
            return runner.Run(runner.Find(feature)).ToArray();
        }

        public IMessageSinkMessage[] Run(Type feature, string traitName, string traitValue)
        {
            var runner = this.CreateRunner(feature.GetTypeInfo().Assembly.GetLocalCodeBase());
            var testCases = runner.Find(feature).Where(testCase =>
            {
                return testCase.Traits.TryGetValue(traitName, out var values) && values.Contains(traitValue);
            }).ToList();

            return runner.Run(testCases).ToArray();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Exception exception = null;
                foreach (var runner in this.runners.Reverse())
                {
                    try
                    {
                        runner.Dispose();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                }

                if (exception != null)
                {
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
            }
        }

        private Xunit2 CreateRunner(string assemblyFileName)
        {
            this.runners.Add(new Xunit2(AppDomainSupport.IfAvailable, new NullSourceInformationProvider(), assemblyFileName));
            return this.runners.Last();
        }
    }
}
