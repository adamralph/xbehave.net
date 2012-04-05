// <copyright file="SpecificationPrimitiveFacts.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Legacy
{
    using System;
    using Xbehave.Legacy;
    using Xunit;

    public class SpecificationPrimitiveFacts
    {
        [Fact]
        public void CreatePrimitive_WithNullAction_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new Step<Action>("foo", null));
        }

        [Fact]
        public void CreatePrimitive_WithNullMessage_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new Step<Action>(null, () => { }));
        }
    }
}