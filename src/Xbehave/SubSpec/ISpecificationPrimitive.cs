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
    public interface ISpecificationPrimitive
    {
        /// <summary>
        /// Indicate that execution of this delegate should be canceled after a specified timeout.
        /// </summary>
        /// <param name="timeoutMs">The timeout in milliseconds.</param>
        ISpecificationPrimitive WithTimeout( int timeoutMs );
    }
}