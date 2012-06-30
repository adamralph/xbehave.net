// <copyright file="Issue9.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Issues.Old
{
    using FluentAssertions;

    using Xbehave;
    using Xbehave.Issues;

    /// <summary>
    /// https://bitbucket.org/adamralph/subspecgwt/issue/9/add-given-func
    /// </summary>
    public class Issue9
    {
        [Specification]
        public static void AutonamingWithExplicitDisposalAction()
        {
            var disposable0 = default(ImplicitDisposable);
            var disposable1 = default(ImplicitDisposable);
            var disposable2 = default(ImplicitDisposable);

            "Given some disposables"
                .Given(() =>
                    {
                        disposable0 = new ImplicitDisposable().Using();
                        disposable1 = new ImplicitDisposable().Using();
                        disposable2 = new ImplicitDisposable().Using();
                    });

            "when the disposables are used"
                .When(() =>
                    {
                        disposable0.Use();
                        disposable1.Use();
                        disposable2.Use();
                    })
                .Then(() => true.Should().Be(false)).InIsolation();
        }
    }
}
