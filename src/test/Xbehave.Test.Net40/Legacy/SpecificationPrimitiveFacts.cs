// <copyright file="SpecificationPrimitiveFacts.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Legacy
{
    using System;
    using Xbehave.Sdk;
    using Xunit;

    public static class SpecificationPrimitiveFacts
    {
        [Fact]
        public static void CreatePrimitiveWithNullActionThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new Step("Given", "foo", false, (Action)null));
        }

        [Fact]
        public static void CreatePrimitiveWithNullMessageThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new Step("Given", null, false, (Action)null));
        }
    }
}