// <copyright file="DisposableFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Infra
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FakeItEasy;
    using Xbehave.Infra;

    public static class DisposableFeature
    {
        [Scenario]
        public static void UsingAnAction()
        {
            var action = default(Action);
            var disposable = default(Disposable);

            "Given an action"
                .Given(() => action = A.Fake<Action>());

            "And a Disposable constructing using the action"
                .And(() => disposable = new Disposable(action));

            "When disposing the disposable"
                .When(() => disposable.Dispose());

            "Then the action is invoked"
                .Then(() => A.CallTo(() => action.Invoke()).MustHaveHappened(Repeated.Exactly.Once));
        }

        [Scenario]
        public static void UsingManyDisposableImplementers()
        {
            var disposableImplementer1 = default(IDisposable);
            var disposableImplementer2 = default(IDisposable);
            var disposable = default(Disposable);

            "Given an disposable implementer"
                .Given(() => disposableImplementer1 = A.Fake<IDisposable>());

            "and another disposable implementer"
                .And(() => disposableImplementer2 = A.Fake<IDisposable>());

            "And a disposable constructing using the disposable implementers"
                .And(() => disposable = new Disposable(new[] { disposableImplementer1, disposableImplementer2 }));

            "When disposing the disposable"
                .When(() => disposable.Dispose());

            "Then the first disposable implementer is disposed"
                .Then(() => A.CallTo(() => disposableImplementer1.Dispose()).MustHaveHappened(Repeated.Exactly.Once));

            "and the second disposable implementer is disposed"
                .And(() => A.CallTo(() => disposableImplementer2.Dispose()).MustHaveHappened(Repeated.Exactly.Once));
        }
    }
}
