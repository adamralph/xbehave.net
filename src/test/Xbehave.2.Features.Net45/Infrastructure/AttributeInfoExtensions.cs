// <copyright file="AttributeInfoExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance.Infrastructure
{
    using System.Collections.Generic;
    using Xunit.Abstractions;

    internal static class AttributeInfoExtensions
    {
        public static IEnumerable<object> GetExampleValues(this IAttributeInfo info)
        {
            return info.GetConstructorArguments();
        }
    }
}
