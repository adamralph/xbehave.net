// <copyright file="StepTest.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Xunit.Sdk;

    public class StepTest : XunitTest
    {
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "It's a constructor.")]
        public StepTest(
            IXunitTestCase testCase, string scenarioName, int scenarioNumber, int stepNumber, string stepName)
            : base(
                testCase,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} [{1}.{2}] {3}",
                    scenarioName,
                    scenarioNumber.ToString("D2", CultureInfo.InvariantCulture),
                    stepNumber.ToString("D2", CultureInfo.InvariantCulture),
                    stepName))
        {
        }
    }
}
