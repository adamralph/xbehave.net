// <copyright file="ScenarioTestGroup.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioTestGroup : IScenarioTestGroup
    {
        private readonly IXunitTestCase testCase;
        private readonly string displayName;
        private readonly int scenarioNumber;
        private readonly Type testClass;
        private readonly MethodInfo testMethod;
        private readonly object[] testMethodArguments;
        private readonly string skipReason;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterTestGroupAttributes;

        public ScenarioTestGroup(
            IXunitTestCase testCase,
            string displayName,
            int scenarioNumber,
            Type testClass,
            MethodInfo testMethod,
            object[] testMethodArguments,
            string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterTestGroupAttributes)
        {
            this.testCase = testCase;
            this.displayName = displayName;
            this.scenarioNumber = scenarioNumber;
            this.testClass = testClass;
            this.testMethod = testMethod;
            this.testMethodArguments = testMethodArguments;
            this.skipReason = skipReason;
            this.beforeAfterTestGroupAttributes = beforeAfterTestGroupAttributes;
        }

        public IXunitTestCase TestCase
        {
            get { return this.testCase; }
        }

        public string DisplayName
        {
            get { return this.displayName; }
        }

        ITestCase ITestGroup.TestCase
        {
            get { return this.TestCase; }
        }

        protected int ScenarioNumber
        {
            get { return this.scenarioNumber; }
        }

        protected Type TestClass
        {
            get { return this.testClass; }
        }

        protected MethodInfo TestMethod
        {
            get { return this.testMethod; }
        }

        protected object[] TestMethodArguments
        {
            get { return this.testMethodArguments; }
        }

        protected string SkipReason
        {
            get { return this.skipReason; }
        }

        protected IReadOnlyList<BeforeAfterTestAttribute> BeforeAfterTestGroupAttributes
        {
            get { return this.beforeAfterTestGroupAttributes; }
        }

        public virtual Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            return new ScenarioTestGroupRunner(
                    this.scenarioNumber,
                    this,
                    messageBus,
                    this.testClass,
                    constructorArguments,
                    this.testMethod,
                    this.testMethodArguments,
                    this.skipReason,
                    this.beforeAfterTestGroupAttributes,
                    aggregator,
                    cancellationTokenSource)
                .RunAsync();
        }
    }
}
