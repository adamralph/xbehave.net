// <copyright file="StepTest.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class StepTest : LongLivedMarshalByRefObject, IStepTest
    {
        private readonly IScenarioTestGroup testGroup;
        private readonly int stepNumber;
        private readonly string stepName;
        private readonly string displayName;

        public StepTest(
            IScenarioTestGroup testGroup, int stepNumber, string stepText, IEnumerable<object> testMethodArguments)
            : this(testGroup, stepNumber, GetStepName(stepText, testMethodArguments))
        {
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "It's a constructor.")]
        public StepTest(IScenarioTestGroup testGroup, int stepNumber, string stepName)
        {
            Guard.AgainstNullArgument("testGroup", testGroup);

            this.testGroup = testGroup;
            this.stepNumber = stepNumber;
            this.stepName = stepName;
            this.displayName = string.Format(
                CultureInfo.InvariantCulture,
                "{0} [{1}] {2}",
                testGroup.DisplayName,
                stepNumber.ToString("D2", CultureInfo.InvariantCulture),
                stepName);
        }

        public IScenarioTestGroup TestGroup
        {
            get { return this.testGroup; }
        }

        public int StepNumber
        {
            get { return this.stepNumber; }
        }

        public string StepName
        {
            get { return this.stepName; }
        }

        public string DisplayName
        {
            get { return this.displayName; }
        }

        public ITestCase TestCase
        {
            get { return this.testGroup.TestCase; }
        }

        private static string GetStepName(string stepText, IEnumerable<object> testMethodArguments)
        {
            try
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    stepText ?? string.Empty,
                    (testMethodArguments ?? Enumerable.Empty<object>()).Select(argument => argument ?? "null").ToArray());
            }
            catch (FormatException)
            {
                return stepText;
            }
        }
    }
}
