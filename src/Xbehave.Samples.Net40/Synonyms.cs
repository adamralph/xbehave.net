// <copyright file="Synonyms.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Samples.Net40
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Xbehave.Fluent;
    using Xbehave.Samples.Fixtures;

    public static class Synonyms
    {
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        public static IStep x(this string text, Action body)
        {
            return text.f(body);
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        public static IStep ʃ(this string text, Action body)
        {
            return text.f(body);
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        public static IStep σʃσ(this string text, Action body)
        {
            return text.f(body);
        }

        public static IStep 梟(this string text, Action body)
        {
            return text.f(body);
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        public static IStep χ(this string text, Action body)
        {
            return text.f(body);
        }

        [Scenario]
        public static void Addition(int x, int y, Calculator calculator, int answer)
        {
            "Given the number 1"
                .x(() => x = 1);

            "And the number 2"
                .ʃ(() => y = 2);

            "And a calculator"
                .σʃσ(() => calculator = new Calculator());

            "When I add the numbers together"
                .梟(() => answer = calculator.Add(x, y));

            "Then the answer is 3"
                .χ(() => answer.Should().Be(3));
        }
    }
}
