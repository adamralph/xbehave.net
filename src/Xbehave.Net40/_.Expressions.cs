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
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Given(Expression<Action> step)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().Given(step.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition Given(Expression<Func<IDisposable>> step)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().Given(step.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition Given(Expression<Func<IEnumerable<IDisposable>>> step)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().Given(step.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="dispose">The dispose.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Given(Expression<Action> step, Action dispose)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().Given(step.Compile(), dispose);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition When(Expression<Action> step)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().When(step.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition When(Expression<Func<IDisposable>> step)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().When(step.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition When(Expression<Func<IEnumerable<IDisposable>>> step)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().When(step.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="dispose">The dispose.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition When(Expression<Action> step, Action dispose)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().When(step.Compile(), dispose);
        }
        
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition Then(Expression<Action> step)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().Then(step.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition ThenInIsolation(Expression<Action> step)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().ThenInIsolation(step.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="reason">The reason.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        public static IStepDefinition ThenSkip(Expression<Action> step, string reason)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().ThenSkip(step.Compile(), reason);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition And(Expression<Action> step)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().And(step.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition And(Expression<Func<IDisposable>> step)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().And(step.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By design.")]
        public static IStepDefinition And(Expression<Func<IEnumerable<IDisposable>>> step)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().And(step.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="dispose">The dispose.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition And(Expression<Action> step, Action dispose)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().And(step.Compile(), dispose);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition AndInIsolation(Expression<Action> step)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().AndInIsolation(step.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="reason">The reason.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        public static IStepDefinition AndSkip(Expression<Action> step, string reason)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().AndSkip(step.Compile(), reason);
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition But(Expression<Action> step)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().But(step.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        public static IStepDefinition ButInIsolation(Expression<Action> step)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().ButInIsolation(step.Compile());
        }

        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="reason">The reason.</param>
        /// <returns>
        /// An instance of <see cref="IStepDefinition"/>.
        /// </returns>
        /// <remarks>
        /// This is the equivalent of <see cref="Xunit.FactAttribute.Skip"/>.
        /// E.g. <code>[Fact(Skip = "Work in progress.")]</code>.
        /// </remarks>
        public static IStepDefinition ButSkip(Expression<Action> step, string reason)
        {
            Require.NotNull(step, "step");
            return step.Body.ToStepName().ButSkip(step.Compile(), reason);
        }
    }
}
