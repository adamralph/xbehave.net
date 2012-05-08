// <copyright file="_.Expressions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using Xbehave.ExpressionNaming;
    using Xbehave.Fluent;
    using Xbehave.Infra;

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
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Given(Expression<Action> body)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().Given(body.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition Given(Expression<Func<IDisposable>> body)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().Given(body.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition Given(Expression<Func<IEnumerable<IDisposable>>> body)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().Given(body.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <param name="dispose">The dispose.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Given(Expression<Action> body, Action dispose)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().Given(body.Compile(), dispose);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition When(Expression<Action> body)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().When(body.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition When(Expression<Func<IDisposable>> body)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().When(body.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition When(Expression<Func<IEnumerable<IDisposable>>> body)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().When(body.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <param name="dispose">The dispose.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition When(Expression<Action> body, Action dispose)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().When(body.Compile(), dispose);
        }
        
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Then(Expression<Action> body)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().Then(body.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition ThenInIsolation(Expression<Action> body)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().ThenInIsolation(body.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <param name="reason">The reason.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        public static IStepDefinition ThenSkip(Expression<Action> body, string reason)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().ThenSkip(body.Compile(), reason);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition And(Expression<Action> body)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().And(body.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition And(Expression<Func<IDisposable>> body)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().And(body.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition And(Expression<Func<IEnumerable<IDisposable>>> body)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().And(body.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <param name="dispose">The dispose.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition And(Expression<Action> body, Action dispose)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().And(body.Compile(), dispose);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition AndInIsolation(Expression<Action> body)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().AndInIsolation(body.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <param name="reason">The reason.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        public static IStepDefinition AndSkip(Expression<Action> body, string reason)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().AndSkip(body.Compile(), reason);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition But(Expression<Action> body)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().But(body.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition ButInIsolation(Expression<Action> body)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().ButInIsolation(body.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="body">The body of the step.</param>
        /// <param name="reason">The reason.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        public static IStepDefinition ButSkip(Expression<Action> body, string reason)
        {
            Require.NotNull(body, "body");
            return body.Body.ToSentence().ButSkip(body.Compile(), reason);
        }
    }
}
