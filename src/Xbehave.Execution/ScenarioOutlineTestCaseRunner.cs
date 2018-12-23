// https://github.com/xunit/xunit/blob/2.4.1/src/xunit.execution/Sdk/Frameworks/Runners/XunitTheoryTestCaseRunner.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xbehave.Execution
{
    /// <summary>
    /// The test case runner for xUnit.net v2 theories (which could not be pre-enumerated;
    /// pre-enumerated test cases use <see cref="XunitTestCaseRunner"/>).
    /// </summary>
    public class ScenarioOutlineTestCaseRunner : XunitTestCaseRunner
    {
        static readonly object[] NoArguments = new object[0];

        readonly ExceptionAggregator cleanupAggregator = new ExceptionAggregator();
        Exception dataDiscoveryException;
        readonly List<ScenarioRunner> testRunners = new List<ScenarioRunner>();
        readonly List<IDisposable> toDispose = new List<IDisposable>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ScenarioOutlineTestCaseRunner"/> class.
        /// </summary>
        /// <param name="testCase">The test case to be run.</param>
        /// <param name="displayName">The display name of the test case.</param>
        /// <param name="skipReason">The skip reason, if the test is to be skipped.</param>
        /// <param name="constructorArguments">The arguments to be passed to the test class constructor.</param>
        /// <param name="diagnosticMessageSink">The message sink used to send diagnostic messages</param>
        /// <param name="messageBus">The message bus to report run status to.</param>
        /// <param name="aggregator">The exception aggregator used to run code and collect exceptions.</param>
        /// <param name="cancellationTokenSource">The task cancellation token source, used to cancel the test run.</param>
        public ScenarioOutlineTestCaseRunner(IXunitTestCase testCase,
                                         string displayName,
                                         string skipReason,
                                         object[] constructorArguments,
                                         IMessageSink diagnosticMessageSink,
                                         IMessageBus messageBus,
                                         ExceptionAggregator aggregator,
                                         CancellationTokenSource cancellationTokenSource)
            : base(testCase, displayName, skipReason, constructorArguments, NoArguments, messageBus, aggregator, cancellationTokenSource)
        {
            DiagnosticMessageSink = diagnosticMessageSink;
        }

        /// <summary>
        /// Gets the message sink used to report <see cref="IDiagnosticMessage"/> messages.
        /// </summary>
        protected IMessageSink DiagnosticMessageSink { get; }

        /// <inheritdoc/>
        protected override async Task AfterTestCaseStartingAsync()
        {
            await base.AfterTestCaseStartingAsync();

            try
            {
                var dataAttributes = TestCase.TestMethod.Method.GetCustomAttributes(typeof(DataAttribute));

                foreach (var dataAttribute in dataAttributes)
                {
                    var discovererAttribute = dataAttribute.GetCustomAttributes(typeof(DataDiscovererAttribute)).First();
                    var args = discovererAttribute.GetConstructorArguments().Cast<string>().ToList();
                    var discovererType = SerializationHelper.GetType(args[1], args[0]);
                    if (discovererType == null)
                    {
                        if (dataAttribute is IReflectionAttributeInfo reflectionAttribute)
                            Aggregator.Add(new InvalidOperationException($"Data discoverer specified for {reflectionAttribute.Attribute.GetType()} on {TestCase.TestMethod.TestClass.Class.Name}.{TestCase.TestMethod.Method.Name} does not exist."));
                        else
                            Aggregator.Add(new InvalidOperationException($"A data discoverer specified on {TestCase.TestMethod.TestClass.Class.Name}.{TestCase.TestMethod.Method.Name} does not exist."));

                        continue;
                    }

                    IDataDiscoverer discoverer;
                    try
                    {
                        discoverer = ExtensibilityPointFactory.GetDataDiscoverer(DiagnosticMessageSink, discovererType);
                    }
                    catch (InvalidCastException)
                    {
                        if (dataAttribute is IReflectionAttributeInfo reflectionAttribute)
                            Aggregator.Add(new InvalidOperationException($"Data discoverer specified for {reflectionAttribute.Attribute.GetType()} on {TestCase.TestMethod.TestClass.Class.Name}.{TestCase.TestMethod.Method.Name} does not implement IDataDiscoverer."));
                        else
                            Aggregator.Add(new InvalidOperationException($"A data discoverer specified on {TestCase.TestMethod.TestClass.Class.Name}.{TestCase.TestMethod.Method.Name} does not implement IDataDiscoverer."));

                        continue;
                    }

                    var data = discoverer.GetData(dataAttribute, TestCase.TestMethod.Method);
                    if (data == null)
                    {
                        Aggregator.Add(new InvalidOperationException($"Test data returned null for {TestCase.TestMethod.TestClass.Class.Name}.{TestCase.TestMethod.Method.Name}. Make sure it is statically initialized before this test method is called."));
                        continue;
                    }

                    foreach (var dataRow in data)
                    {
                        toDispose.AddRange(dataRow.OfType<IDisposable>());

                        var info = new ScenarioInfo(TestCase.TestMethod.Method, dataRow, DisplayName);
                        var methodToRun = info.MethodToRun;
                        var convertedDataRow = info.ConvertedDataRow.ToArray();

                        var theoryDisplayName = info.ScenarioDisplayName;
                        var test = CreateTest(TestCase, theoryDisplayName);
                        var skipReason = SkipReason ?? dataAttribute.GetNamedArgument<string>("Skip");
                        testRunners.Add(CreateTestRunner(test, MessageBus, TestClass, ConstructorArguments, methodToRun, convertedDataRow, skipReason, BeforeAfterAttributes, Aggregator, CancellationTokenSource));
                    }
                }

                if (!this.testRunners.Any())
                {
                    var info = new ScenarioInfo(TestCase.TestMethod.Method, NoArguments, DisplayName);
                    var methodToRun = info.MethodToRun;
                    var convertedDataRow = info.ConvertedDataRow.ToArray();

                    var theoryDisplayName = info.ScenarioDisplayName;
                    var test = CreateTest(TestCase, theoryDisplayName);
                    this.testRunners.Add(CreateTestRunner(test, MessageBus, TestClass, ConstructorArguments, methodToRun, convertedDataRow, SkipReason, BeforeAfterAttributes, Aggregator, CancellationTokenSource));
                }
            }
            catch (Exception ex)
            {
                // Stash the exception so we can surface it during RunTestAsync
                dataDiscoveryException = ex;
            }
        }

        // https://github.com/xunit/xunit/blob/2.4.1/src/xunit.execution/Sdk/Frameworks/Runners/XunitTestCaseRunner.cs#L101-L102
        new static Scenario CreateTest(IXunitTestCase testCase, string displayName)
            => new Scenario(testCase, displayName);

        // https://github.com/xunit/xunit/blob/2.4.1/src/xunit.execution/Sdk/Frameworks/Runners/XunitTestCaseRunner.cs#L107-L118
        static ScenarioRunner CreateTestRunner(Sdk.IScenario test,
                                               IMessageBus messageBus,
                                               Type testClass,
                                               object[] constructorArguments,
                                               System.Reflection.MethodInfo testMethod,
                                               object[] testMethodArguments,
                                               string skipReason,
                                               IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes,
                                               ExceptionAggregator aggregator,
                                               CancellationTokenSource cancellationTokenSource)
            => new ScenarioRunner(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments,
                                  skipReason, beforeAfterAttributes, new ExceptionAggregator(aggregator), cancellationTokenSource);

        /// <inheritdoc/>
        protected override Task BeforeTestCaseFinishedAsync()
        {
            Aggregator.Aggregate(cleanupAggregator);

            return base.BeforeTestCaseFinishedAsync();
        }

        /// <inheritdoc/>
        protected override async Task<RunSummary> RunTestAsync()
        {
            if (dataDiscoveryException != null)
                return RunTest_DataDiscoveryException();

            var runSummary = new RunSummary();
            foreach (var testRunner in testRunners)
                runSummary.Aggregate(await testRunner.RunAsync());

            // Run the cleanup here so we can include cleanup time in the run summary,
            // but save any exceptions so we can surface them during the cleanup phase,
            // so they get properly reported as test case cleanup failures.
            var timer = new ExecutionTimer();
            foreach (var disposable in toDispose)
                timer.Aggregate(() => cleanupAggregator.Run(disposable.Dispose));

            runSummary.Time += timer.Total;
            return runSummary;
        }

        RunSummary RunTest_DataDiscoveryException()
        {
            var test = new XunitTest(TestCase, DisplayName);

            if (!MessageBus.QueueMessage(new TestStarting(test)))
                CancellationTokenSource.Cancel();
            else if (!MessageBus.QueueMessage(new TestFailed(test, 0, null, dataDiscoveryException.Unwrap())))
                CancellationTokenSource.Cancel();
            if (!MessageBus.QueueMessage(new TestFinished(test, 0, null)))
                CancellationTokenSource.Cancel();

            return new RunSummary { Total = 1, Failed = 1 };
        }
    }
}
