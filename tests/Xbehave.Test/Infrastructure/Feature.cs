using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Xbehave.Test.Infrastructure
{
    public abstract class Feature : IDisposable
    {
        private static readonly TestAssemblyConfiguration config = new TestAssemblyConfiguration { AppDomain = AppDomainSupport.Denied, };

        private readonly Dictionary<Assembly, Xunit2> runners = new Dictionary<Assembly, Xunit2>();

        ~Feature()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public TMessage[] Run<TMessage>(Type feature)
            where TMessage : IMessageSinkMessage =>
            this.Run(feature).OfType<TMessage>().ToArray();

        public IMessageSinkMessage[] Run(Type feature)
        {
            var runner = this.InternRunner(feature.Assembly);
            return runner.Run(runner.Find(feature, config), config).ToArray();
        }

        public TMessage[] Run<TMessage>(Assembly assembly, string collectionName)
            where TMessage : IMessageSinkMessage
        {
            var runner = this.InternRunner(assembly);
            return runner.Run(runner.Find(collectionName, config), config).OfType<TMessage>().ToArray();
        }

        public TMessage[] Run<TMessage>(Type feature, string traitName, string traitValue)
            where TMessage : IMessageSinkMessage
        {
            var runner = this.InternRunner(feature.Assembly);
            return runner.Run(runner.Find(feature, traitName, traitValue, config), config).OfType<TMessage>().ToArray();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var runner in this.runners.Values)
                {
                    runner.Dispose();
                }
            }
        }

        private Xunit2 InternRunner(Assembly assembly)
        {
            if (!this.runners.TryGetValue(assembly, out var runner))
            {
                runner = new Xunit2(AppDomainSupport.Denied, new NullSourceInformationProvider(), assembly.GetFileName());
                this.runners.Add(assembly, runner);
            }

            return runner;
        }
    }
}
