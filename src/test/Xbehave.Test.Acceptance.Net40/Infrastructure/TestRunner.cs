// <copyright file="TestRunner.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Sdk;

    internal static class TestRunner
    {
        public static IEnumerable<MethodResult> Run(Type featureDefinition)
        {
            var feature = new TestClassCommand(featureDefinition);
            return TestClassCommandRunner.Execute(feature, feature.EnumerateTestMethods().ToList(), startCallback: null, resultCallback: null).Results
                .OfType<MethodResult>();
        }
    }
}
