using System;
using System.Collections.Generic;
using Xunit.Sdk;
using Xunit.Extensions;

namespace SubSpec
{
    [AttributeUsage( AttributeTargets.Method, AllowMultiple = false, Inherited = true )]
    public class ThesisAttribute : TheoryAttribute
    {
        protected override IEnumerable<ITestCommand> EnumerateTestCommands( IMethodInfo method )
        {
            // prepare specification invocations  
            var theoryTestCommands = base.EnumerateTestCommands( method );

            var commands = new List<ITestCommand>();

            foreach (var item in theoryTestCommands)
            {
                if (item is TheoryCommand)
                {
                    commands.AddRange( SubSpec.Core.SpecificationContext.SafelyEnumerateTestCommands( method,
                        m =>
                        {
                            // Create a new test class instance
                            object obj = item.ShouldCreateInstance ? Activator.CreateInstance( method.MethodInfo.ReflectedType ) : null;
                            
                            // create method
                            item.Execute( obj );
                        } ) );

                }
                else
                {
                    commands.Clear();
                    commands.Add( item );
                    break;
                }
            }

            return commands;
        }
    }
}
