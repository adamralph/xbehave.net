// <copyright file="Step.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Fluent
{
    using System;

    internal partial class Step : IStep
    {
        [Obsolete("Use When().Teardown() instead.")]
        public IStep When(string message, Action body, Action dispose)
        {
            return message.When(body).Teardown(dispose);
        }

        [Obsolete("Use When().Teardown() instead.")]
        public IStep Then(string message, Action body, Action dispose)
        {
            return message.Then(body).Teardown(dispose);
        }

        [Obsolete("Use When().Teardown() instead.")]
        public IStep And(string message, Action body, Action dispose)
        {
            return message.And(body).Teardown(dispose);
        }

        [Obsolete("Use When().Teardown() instead.")]
        public IStep But(string message, Action body, Action dispose)
        {
            return message.But(body).Teardown(dispose);
        }
    }
}
