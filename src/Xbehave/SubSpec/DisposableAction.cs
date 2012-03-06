using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Sdk;
using System.Xml;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace Xbehave
{
    public class DisposableAction : IDisposable
    {
        public static readonly DisposableAction None = new DisposableAction( () => { } );

        private readonly Action _action;

        public DisposableAction( Action action )
        {
            if (action == null)
                throw new ArgumentNullException( "action" );

            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
}