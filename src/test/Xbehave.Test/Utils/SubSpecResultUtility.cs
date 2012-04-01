using System;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using Xunit;

namespace TestUtility
{
    public static class SubSpecResultUtility
    {
        public static XmlNode GetClassResult( XmlNode assemblyNode, int classIndex )
        {
            XmlNodeList classNodes = assemblyNode.SelectNodes( "class" );
            if (classNodes.Count <= classIndex)
                throw new ArgumentException( "Could not find class item with index " + classIndex + " in XML:\r\n" + assemblyNode.OuterXml );

            return classNodes[classIndex];
        }

        public static XmlNode GetObservationResult( XmlNode assemblyNode, int observationIndex )
        {
            return ObservationResults( assemblyNode ).ElementAt( observationIndex + 1 );
        }

        public static XmlNode ObservationTeardownResult( this XmlNode assemblyNode )
        {
            return GetClassResult( assemblyNode, 0 ).ChildNodes.Cast<XmlNode>().Single( x => x.Attributes.GetNamedItem( "name" ).Value.StartsWith( "}" ) );
        }

        public static XmlNode ObservationSetupResult( this XmlNode assemblyNode )
        {
            return GetClassResult( assemblyNode, 0 ).ChildNodes.Cast<XmlNode>().Single( x => x.Attributes.GetNamedItem( "name" ).Value.StartsWith( "{" ) );
        }

        public static IEnumerable<XmlNode> ObservationResults( this XmlNode assemblyNode )
        {
            return GetClassResult( assemblyNode, 0 ).ChildNodes.Cast<XmlNode>()
                .SkipWhile( x => !IsObservationSetupNode( x ) ).Skip( 1 )
                .TakeWhile( x => !IsObservationTeardownNode( x ) )
                .Select( x => x );
        }

        private static bool IsObservationSetupNode( XmlNode x )
        {
            return x.Attributes.GetNamedItem( "name" ).Value.StartsWith( "{" );
        }
        private static bool IsObservationTeardownNode( XmlNode x )
        {
            return x.Attributes.GetNamedItem( "name" ).Value.StartsWith( "}" );
        }
        public static IEnumerable<XmlNode> AssertionResults( this XmlNode assemblyNode )
        {
            return GetClassResult( assemblyNode, 0 ).ChildNodes.Cast<XmlNode>().Except( assemblyNode.ObservationResults() );
        }

        public static XmlNode Second( this IEnumerable<XmlNode> source )
        {
            return source.ElementAt( 1 );
        }
        public static XmlNode First( this IEnumerable<XmlNode> source )
        {
            return source.First<XmlNode>();
        }

        public static void VerifyOutput( XmlNode testNode, string expectedOutput )
        {
            XmlNode outputNode = testNode.SelectSingleNode( "output" );

            Assert.Equal( expectedOutput, outputNode.InnerXml.Trim() );
        }

        public static void VerifyPassed( XmlNode testNode )
        {
            ResultXmlUtility.AssertAttribute( testNode, "result", "Pass" );
        }

        public static void VerifyFailed( XmlNode testNode )
        {
            ResultXmlUtility.AssertAttribute( testNode, "result", "Fail" );
        }

        public static void VerifyFailedWith( XmlNode testNode, string expectedFailure )
        {
            VerifyFailed( testNode );

            XmlNode firstFailureNode = testNode.SelectSingleNode( "failure" );

            string innerxml = firstFailureNode.SelectSingleNode( "message" ).InnerXml;
            Assert.True( innerxml.StartsWith( expectedFailure ), String.Format( "\nExpected: {0}\nActual: {1}", expectedFailure, innerxml ) );
        }
    }
}