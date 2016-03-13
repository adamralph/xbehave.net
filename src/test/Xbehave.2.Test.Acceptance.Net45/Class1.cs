using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Xbehave;
using Xunit;

namespace Xbehave.Test.Acceptance
{
    public class Class1 : IClassFixture<Foo>
    {
        private readonly Foo foo;

        public Class1(Foo foo)
        {
            this.foo = foo;
            //CallContext.LogicalSetData("magic", 123);
        }

        [Fact]
        public async Task Bar()
        {
            Assert.NotNull(this.foo);
            await Task.Yield();
            Assert.Equal(123, CallContext.LogicalGetData("magic"));
        }
    }

    public class Class2
    {
        public Class2()
        {
            CallContext.LogicalSetData("magic", 123);
        }

        [Background]
        public void Background()
        {
            //CallContext.LogicalSetData("magic", 123);
            //Assert.Equal(123, CallContext.LogicalGetData("magic"));
        }

        [Scenario]
        public void Foo()
        {
            //CallContext.LogicalSetData("magic", 123);
            //Assert.Equal(123, CallContext.LogicalGetData("magic"));

            "blah"
                .x(() => Assert.Equal(123, CallContext.LogicalGetData("magic")));
        }
    }

    public class Foo // : IAsyncLifetime
    {
        public Foo()
        {
            CallContext.LogicalSetData("magic", 123);
        }

        //public async Task DisposeAsync()
        //{
        //    await Task.Yield();
        //}

        //public async Task InitializeAsync()
        //{
        //    await Task.Yield();
        //}
    }
}
