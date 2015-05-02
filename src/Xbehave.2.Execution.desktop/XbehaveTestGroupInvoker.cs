// <copyright file="XbehaveTestGroupInvoker.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class XbehaveTestGroupInvoker
    {
        private readonly ITest testGroup;
        private readonly IMessageBus messageBus;
        private readonly Type testClass;
        private readonly object[] constructorArguments;
        private readonly MethodInfo testMethod;
        private readonly object[] testMethodArguments;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterTestGroupAttributes;
        private readonly ExceptionAggregator aggregator;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly ExecutionTimer timer = new ExecutionTimer();
        private readonly Stack<BeforeAfterTestAttribute> beforeAfterTestGroupAttributesRun =
            new Stack<BeforeAfterTestAttribute>();

        public XbehaveTestGroupInvoker(
            ITest testGroup,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterTestGroupAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            Guard.AgainstNullArgument("testGroup", testGroup);
            Guard.AgainstNullArgument("messageBus", messageBus);
            Guard.AgainstNullArgument("testClass", testClass);
            Guard.AgainstNullArgument("testMethod", testMethod);
            Guard.AgainstNullArgument("beforeAfterTestGroupAttributes", beforeAfterTestGroupAttributes);
            Guard.AgainstNullArgument("aggregator", aggregator);
            Guard.AgainstNullArgument("cancellationTokenSource", cancellationTokenSource);

            this.testGroup = testGroup;
            this.messageBus = messageBus;
            this.testClass = testClass;
            this.constructorArguments = constructorArguments;
            this.testMethod = testMethod;
            this.testMethodArguments = testMethodArguments;
            this.beforeAfterTestGroupAttributes = beforeAfterTestGroupAttributes;
            this.aggregator = aggregator;
            this.cancellationTokenSource = cancellationTokenSource;
        }

        protected ITest TestGroup
        {
            get { return this.testGroup; }
        }

        protected IMessageBus MessageBus
        {
            get { return this.messageBus; }
        }

        protected Type TestClass
        {
            get { return this.testClass; }
        }

        protected IReadOnlyList<object> ConstructorArguments
        {
            get { return this.constructorArguments; }
        }

        protected MethodInfo TestMethod
        {
            get { return this.testMethod; }
        }

        protected IReadOnlyList<object> TestMethodArguments
        {
            get { return this.testMethodArguments; }
        }

        protected IReadOnlyList<BeforeAfterTestAttribute> BeforeAfterTestGroupAttributes
        {
            get { return this.beforeAfterTestGroupAttributes; }
        }

        protected ExceptionAggregator Aggregator
        {
            get { return this.aggregator; }
        }

        protected CancellationTokenSource CancellationTokenSource
        {
            get { return this.cancellationTokenSource; }
        }

        protected ExecutionTimer Timer
        {
            get { return this.timer; }
        }

        public async Task<RunSummary> RunAsync()
        {
            var summary = new RunSummary();
            await this.aggregator.RunAsync(async () =>
            {
                if (!cancellationTokenSource.IsCancellationRequested)
                {
                    var testClassInstance = CreateTestClass();

                    if (!cancellationTokenSource.IsCancellationRequested)
                    {
                        await BeforeTestMethodInvokedAsync();

                        if (!this.cancellationTokenSource.IsCancellationRequested && !this.aggregator.HasExceptions)
                        {
                            summary.Aggregate(await InvokeTestMethodAsync(testClassInstance));
                        }

                        await AfterTestMethodInvokedAsync();
                    }

                    var disposable = testClassInstance as IDisposable;
                    if (disposable != null)
                    {
                        timer.Aggregate(() => aggregator.Run(disposable.Dispose));
                    }
                }

                summary.Time += this.timer.Total;
            });

            return summary;
        }

        protected virtual object CreateTestClass()
        {
            object testClass = null;

            if (!this.testMethod.IsStatic && !this.aggregator.HasExceptions)
            {
                this.timer.Aggregate(() => testClass = Activator.CreateInstance(this.testClass, this.constructorArguments));
            }

            return testClass;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions are collected in the aggregator.")]
        protected virtual Task BeforeTestMethodInvokedAsync()
        {
            foreach (var beforeAfterAttribute in this.beforeAfterTestGroupAttributes)
            {
                try
                {
                    this.timer.Aggregate(() => beforeAfterAttribute.Before(this.testMethod));
                    this.beforeAfterTestGroupAttributesRun.Push(beforeAfterAttribute);
                }
                catch (Exception ex)
                {
                    this.aggregator.Add(ex);
                    break;
                }

                if (this.cancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }
            }

            return Task.FromResult(0);
        }

        protected virtual Task AfterTestMethodInvokedAsync()
        {
            foreach (var beforeAfterAttribute in this.beforeAfterTestGroupAttributesRun)
            {
                this.aggregator.Run(() => this.timer.Aggregate(() => beforeAfterAttribute.After(this.testMethod)));
            }

            return Task.FromResult(0);
        }

        protected async virtual Task<RunSummary> InvokeTestMethodAsync(object testClassInstance)
        {
            await this.aggregator.RunAsync(async () =>
            {
                using (ThreadStaticStepHub.CreateBackgroundSteps())
                {
                    foreach (var backgroundMethod in this.testGroup.TestCase.TestMethod.Method.Type
                        .GetMethods(false)
                        .Where(candidate => candidate.GetCustomAttributes(typeof(BackgroundAttribute)).Any())
                        .Select(method => method.ToRuntimeMethod()))
                    {
                        await this.timer.AggregateAsync(() =>
                            backgroundMethod.InvokeAsync(testClassInstance, null));
                    }
                }

                await this.timer.AggregateAsync(() =>
                    this.testMethod.InvokeAsync(testClassInstance, this.testMethodArguments));
            });

            var runSummary = new RunSummary();
            if (!this.aggregator.HasExceptions)
            {
                runSummary.Aggregate(await this.InvokeStepsAsync(ThreadStaticStepHub.RemoveAll()));
            }

            return runSummary;
        }

        protected async virtual Task<RunSummary> InvokeStepsAsync(IList<StepDefinition> steps)
        {
            var summary = new RunSummary();
            string skipReason = null;
            var testRunners = new List<XbehaveTestRunner>();
            foreach (var item in steps.Select((step, index) => new { step, index }))
            {
                item.step.SkipReason = item.step.SkipReason ?? skipReason;

                var testDisplayName = GetTestDisplayName(
                    this.testGroup.DisplayName, item.index + 1, item.step.Text, this.testMethodArguments);

                var test = new XbehaveTest(this.testGroup, testDisplayName);

                var interceptingBus = new DelegatingMessageBus(
                    this.messageBus,
                    message =>
                    {
                        if (message is ITestFailed && !item.step.ContinueOnFailure)
                        {
                            skipReason = string.Format(
                                CultureInfo.InvariantCulture,
                                "Failed to execute preceding step: {0}",
                                test.DisplayName);
                        }
                    });

                var testRunner = new XbehaveTestRunner(
                    item.step,
                    test,
                    interceptingBus,
                    this.testClass,
                    this.constructorArguments,
                    this.testMethod,
                    this.testMethodArguments,
                    item.step.SkipReason,
                    this.beforeAfterTestGroupAttributes,
                    new ExceptionAggregator(this.aggregator),
                    this.cancellationTokenSource);

                testRunners.Add(testRunner);
                summary.Aggregate(await testRunner.RunAsync());
            }

            var teardowns = testRunners.SelectMany(testRunner => testRunner.Teardowns).ToArray();
            if (teardowns.Any())
            {
                var teardownTimer = new ExecutionTimer();
                var teardownAggregator = new ExceptionAggregator();
                foreach (var teardown in teardowns.Reverse())
                {
                    teardownTimer.Aggregate(() => teardownAggregator.Run(() => teardown()));
                }

                summary.Time += teardownTimer.Total;

                if (teardownAggregator.HasExceptions)
                {
                    summary.Failed++;
                    summary.Total++;

                    var testDisplayName = GetTestDisplayName(
                        this.testGroup.DisplayName, steps.Count + 1, "(Teardown)", this.testMethodArguments);

                    this.messageBus.Queue(
                        new XbehaveTest(this.testGroup, testDisplayName),
                        test => new TestFailed(test, teardownTimer.Total, null, teardownAggregator.ToException()),
                        this.cancellationTokenSource);
                }
            }

            return summary;
        }

        private static string GetTestDisplayName(
            string testGroupDisplayName, int testNumber, string testNameFormat, IEnumerable<object> testMethodArguments)
        {
            string testName;
            try
            {
                testName = string.Format(
                    CultureInfo.InvariantCulture,
                    testNameFormat ?? string.Empty,
                    (testMethodArguments ?? Enumerable.Empty<object>()).Select(argument => argument ?? "null").ToArray());
            }
            catch (FormatException)
            {
                testName = testNameFormat;
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} [{1}] {2}",
                testGroupDisplayName,
                testNumber.ToString("D2", CultureInfo.InvariantCulture),
                testName);
        }
    }
}
