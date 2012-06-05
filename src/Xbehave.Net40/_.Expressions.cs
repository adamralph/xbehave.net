// <copyright file="_.Expressions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using Xbehave.Fluent;
    using Xbehave.Infra;
    using Xbehave.Sdk.ExpressionNaming;

    /// <summary>
    /// Provides a scenario step syntax an auto-generated step name.
    /// </summary>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "_", Justification = "By design.")]
    [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "By design.")]
    public static partial class _
    {
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <param name="teardown">The teardown.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Given(Expression<Action> body, Action teardown = null)
        {
            Guard.AgainstNullArgument("body", body);
            Guard.AgainstNullArgumentProperty("body", "Body", body.Body);
            return body.Body.ToSentence().Given(body.Compile(), teardown);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <param name="teardown">The teardown.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition When(Expression<Action> body, Action teardown = null)
        {
            Guard.AgainstNullArgument("body", body);
            Guard.AgainstNullArgumentProperty("body", "Body", body.Body);
            return body.Body.ToSentence().When(body.Compile(), teardown);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <param name="teardown">The teardown.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Then(Expression<Action> body, Action teardown = null)
        {
            Guard.AgainstNullArgument("body", body);
            Guard.AgainstNullArgumentProperty("body", "Body", body.Body);
            return body.Body.ToSentence().Then(body.Compile(), teardown);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <param name="teardown">The teardown.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition And(Expression<Action> body, Action teardown = null)
        {
            Guard.AgainstNullArgument("body", body);
            Guard.AgainstNullArgumentProperty("body", "Body", body.Body);
            return body.Body.ToSentence().And(body.Compile(), teardown);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <param name="teardown">The teardown.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition But(Expression<Action> body, Action teardown = null)
        {
            Guard.AgainstNullArgument("body", body);
            Guard.AgainstNullArgumentProperty("body", "Body", body.Body);
            return body.Body.ToSentence().But(body.Compile(), teardown);
        }
    }
}
