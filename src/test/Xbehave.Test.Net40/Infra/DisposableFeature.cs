// <copyright file="DisposableFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Infra
{
    using System;
    using FakeItEasy;
    using Xbehave.Infra;

    public static class DisposableFeature
    {
        [Scenario]
        public static void Disposing()
        {
            var action = default(Action);
            var disposable = default(Disposable);

            "Given an action"
                .Given(() => action = A.Fake<Action>());

            "And a disposable constructing using the action"
                .And(() => disposable = new Disposable(action), null);

            "When disposing the disposable"
                .When(() => disposable.Dispose());

            "Then the action is invoked"
                .Then(() => A.CallTo(() => action.Invoke()).MustHaveHappened(Repeated.Exactly.Once));
        }
    }
}
