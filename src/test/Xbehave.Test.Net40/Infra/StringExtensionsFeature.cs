// <copyright file="StringExtensionsFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Infra
{
    using System;
    using FakeItEasy;
    using FluentAssertions;
    using Xbehave.Infra;
    using Xunit;

    public static class StringExtensionsFeature
    {
        [Scenario]
        [Example("            a            b            ", "a b")]
        [Example("  \ta\r\rb\n\n", "a b")]
        public static void CompressingWhiteSpace(string text, string expectedResult)
        {
            var result = default(string);

            "Given text \"{0}\""
                .Given(() => { });

            "When compressing white space"
                .When(() => result = text.CompressWhitespace());

            "Then the result should be \"{1}\""
                .Then(() => result.Should().Be(expectedResult));
        }

        [Scenario]
        [Example("Given ", "a foo", "Given a foo")]
        [Example("Given ", "given a foo", "given a foo")]
        [Example("Given ", "n foos", "Given n foos")]
        public static void MergingUsingOrdinalIgnoreCaseComparison(string originalText, string textToMerge, string expectedResult)
        {
            var result = default(string);

            "Given original text \"{0}\""
                .Given(() => { });

            "And text to merge \"{1}\""
                .And(() => { });

            "When merging using ordinal ignore case comparison"
                .When(() => result = originalText.MergeOrdinalIgnoreCase(textToMerge));

            "Then the result should be \"{2}\""
                .Then(() => result.Should().Be(expectedResult));
        }
    }
}
