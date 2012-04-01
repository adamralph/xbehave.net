using System.IO;
using System.Xml;
using System;
using System.Linq;
using System.Collections.Generic;
using Xunit.Extensions;
namespace TestUtility
{
    public class SubSpecAcceptanceTest
    {
        public XmlNode ExecuteSpecification( string code )
        {
            return ExecuteWithAdditionalFilesIncluded( code, "SubSpec.cs" );
        }

        public XmlNode ExecuteThesis( string code )
        {
            return ExecuteWithAdditionalFilesIncluded( code, "SubSpec.cs", "SubSpec.Thesis.cs" );
        }

        private static XmlNode ExecuteWithAdditionalFilesIncluded( string code, params string[] files )
        {
            string codeFile = Guid.NewGuid().ToString( "D" ) + ".cs";

            File.WriteAllText( codeFile, code );
            using (new Xbehave.DisposableAction( () => File.Delete( codeFile ) ))
            {
                using (MockAssembly mockAssembly = new MockAssembly())
                {
                    mockAssembly.Compile( files.Prepend( codeFile ).ToArray(), typeof( TheoryAttribute ).Assembly.Location );
                    return mockAssembly.Run();
                }
            }
        }
    }

    internal static class Mixin
    {
        public static IEnumerable<T> Prepend<T>( this IEnumerable<T> source, T item )
        {
            yield return item;

            foreach (var x in source)
            {
                yield return x;
            }
        }
    }
}