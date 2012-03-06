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
    [AttributeUsage( AttributeTargets.Method, AllowMultiple = false, Inherited = true )]
    public class SpecificationAttribute : FactAttribute
    {
        protected override IEnumerable<ITestCommand> EnumerateTestCommands( IMethodInfo method )
        {
            return Core.SpecificationContext.SafelyEnumerateTestCommands( method, RegisterSpecificationPrimitives);
        }

        public static IEnumerable<ITestCommand> FtoEnumerateTestCommands( IMethodInfo method )
        {
            return Core.SpecificationContext.SafelyEnumerateTestCommands( method, RegisterSpecificationPrimitives );
        }

        private static void RegisterSpecificationPrimitives( IMethodInfo method )
        {
            if (method.IsStatic)
                method.Invoke( null, null );
            else
            {
                ConstructorInfo defaultConstructor = method.MethodInfo.ReflectedType.GetConstructor( Type.EmptyTypes );

                if (defaultConstructor == null)
                    throw new InvalidOperationException( "Specification class does not have a default constructor" );

                object instance = defaultConstructor.Invoke( null );
                method.Invoke( instance, null );
            }
        }
    }
}