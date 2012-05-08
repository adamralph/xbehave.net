// <copyright file="SpecificationPrimitiveFacts.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Legacy
{
    using System;
    using Xbehave.Internal;
    using Xunit;

    public static class SpecificationPrimitiveFacts
    {
        [Fact]
        public static void CreatePrimitiveWithNullActionThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new Step("Given", "foo", (Action)null, false, null));
        }

        [Fact]
        public static void CreatePrimitiveWithNullMessageThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new Step("Given", null, () => (IDisposable)null, false, null));
        }
    }
}