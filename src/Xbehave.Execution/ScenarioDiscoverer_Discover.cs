using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xbehave.Execution
{
    // https://github.com/xunit/xunit/blob/1fb6b672768bed98f5e7b6656c1bbb3792ed0530/src/xunit.execution/Sdk/Frameworks/TheoryDiscoverer.cs#L127-L263
    [GeneratedCode("https://github.com/xunit/xunit", "1fb6b672768bed98f5e7b6656c1bbb3792ed0530")]
    public partial class ScenarioDiscoverer
    {
        public override IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute)
        {
            // Special case Skip, because we want a single Skip (not one per data item); plus, a skipped test may
            // not actually have any data (which is quasi-legal, since it's skipped).
            var skipReason = theoryAttribute.GetNamedArgument<string>("Skip");
            if (skipReason != null)
                return CreateTestCasesForSkip(discoveryOptions, testMethod, theoryAttribute, skipReason);

            if (discoveryOptions.PreEnumerateTheoriesOrDefault())
            {
                try
                {
                    var dataAttributes = testMethod.Method.GetCustomAttributes(typeof(DataAttribute));
                    var results = new List<IXunitTestCase>();

                    foreach (var dataAttribute in dataAttributes)
                    {
                        var discovererAttribute = dataAttribute.GetCustomAttributes(typeof(DataDiscovererAttribute)).First();
                        IDataDiscoverer discoverer;
                        try
                        {
                            discoverer = ExtensibilityPointFactory.GetDataDiscoverer(DiagnosticMessageSink, discovererAttribute);
                        }
                        catch (InvalidCastException)
                        {
                            var reflectionAttribute = dataAttribute as IReflectionAttributeInfo;

                            if (reflectionAttribute != null)
                                results.Add(
                                    new ExecutionErrorTestCase(
                                        DiagnosticMessageSink,
                                        discoveryOptions.MethodDisplayOrDefault(),
                                        discoveryOptions.MethodDisplayOptionsOrDefault(),
                                        testMethod,
                                        $"Data discoverer specified for {reflectionAttribute.Attribute.GetType()} on {testMethod.TestClass.Class.Name}.{testMethod.Method.Name} does not implement IDataDiscoverer."
                                    )
                                );
                            else
                                results.Add(
                                    new ExecutionErrorTestCase(
                                        DiagnosticMessageSink,
                                        discoveryOptions.MethodDisplayOrDefault(),
                                        discoveryOptions.MethodDisplayOptionsOrDefault(),
                                        testMethod,
                                        $"A data discoverer specified on {testMethod.TestClass.Class.Name}.{testMethod.Method.Name} does not implement IDataDiscoverer."
                                    )
                                );

                            continue;
                        }

                        if (discoverer == null)
                        {
                            var reflectionAttribute = dataAttribute as IReflectionAttributeInfo;

                            if (reflectionAttribute != null)
                                results.Add(
                                    new ExecutionErrorTestCase(
                                        DiagnosticMessageSink,
                                        discoveryOptions.MethodDisplayOrDefault(),
                                        discoveryOptions.MethodDisplayOptionsOrDefault(),
                                        testMethod,
                                        $"Data discoverer specified for {reflectionAttribute.Attribute.GetType()} on {testMethod.TestClass.Class.Name}.{testMethod.Method.Name} does not exist."
                                    )
                                );
                            else
                                results.Add(
                                    new ExecutionErrorTestCase(
                                        DiagnosticMessageSink,
                                        discoveryOptions.MethodDisplayOrDefault(),
                                        discoveryOptions.MethodDisplayOptionsOrDefault(),
                                        testMethod,
                                        $"A data discoverer specified on {testMethod.TestClass.Class.Name}.{testMethod.Method.Name} does not exist."
                                    )
                                );

                            continue;
                        }

                        skipReason = dataAttribute.GetNamedArgument<string>("Skip");

                        if (!discoverer.SupportsDiscoveryEnumeration(dataAttribute, testMethod.Method))
                            return CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);

                        var data = discoverer.GetData(dataAttribute, testMethod.Method);
                        if (data == null)
                        {
                            results.Add(
                                new ExecutionErrorTestCase(
                                    DiagnosticMessageSink,
                                    discoveryOptions.MethodDisplayOrDefault(),
                                    discoveryOptions.MethodDisplayOptionsOrDefault(),
                                    testMethod,
                                    $"Test data returned null for {testMethod.TestClass.Class.Name}.{testMethod.Method.Name}. Make sure it is statically initialized before this test method is called."
                                )
                            );

                            continue;
                        }

                        foreach (var dataRow in data)
                        {
                            // Determine whether we can serialize the test case, since we need a way to uniquely
                            // identify a test and serialization is the best way to do that. If it's not serializable,
                            // this will throw and we will fall back to a single theory test case that gets its data at runtime.
                            if (!SerializationHelper.IsSerializable(dataRow))
                            {
                                DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"Non-serializable data ('{dataRow.GetType().FullName}') found for '{testMethod.TestClass.Class.Name}.{testMethod.Method.Name}'; falling back to single test case."));
                                return CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);
                            }

                            var testCases =
                                skipReason != null
                                    ? CreateTestCasesForSkippedDataRow(discoveryOptions, testMethod, theoryAttribute, dataRow, skipReason)
                                    : CreateTestCasesForDataRow(discoveryOptions, testMethod, theoryAttribute, dataRow);

                            results.AddRange(testCases);
                        }
                    }

                    // here's the difference between xunit and xbehave
                    if (results.Count == 0)
                        //results.Add(new ExecutionErrorTestCase(DiagnosticMessageSink,
                        //                                       discoveryOptions.MethodDisplayOrDefault(),
                        //                                       discoveryOptions.MethodDisplayOptionsOrDefault(),
                        //                                       testMethod,
                        //                                       $"No data found for {testMethod.TestClass.Class.Name}.{testMethod.Method.Name}"));
                        return CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);

                    return results;
                }
                catch (Exception ex)    // If something goes wrong, fall through to return just the XunitTestCase
                {
                    DiagnosticMessageSink.OnMessage(new DiagnosticMessage($"Exception thrown during theory discovery on '{testMethod.TestClass.Class.Name}.{testMethod.Method.Name}'; falling back to single test case.{Environment.NewLine}{ex}"));
                }
            }

            return CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);
        }
    }
}
