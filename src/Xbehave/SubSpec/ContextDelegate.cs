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
    /// <remarks>
    /// We declare our own delegate instead of using Func 
    /// because we can't use implicit covariance on the generic type parameter in .net 3.5"/>
    /// </remarks>
    public delegate IDisposable ContextDelegate();
}