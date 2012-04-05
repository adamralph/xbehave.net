// <copyright file="EitherGivenOrWhenFailedException.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Legacy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    // TODO: address ExceptionsShouldBePublic
    [Serializable]
    [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Justification = "Part of the original SubSpec code - will be addressed.")]
    internal class EitherGivenOrWhenFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EitherGivenOrWhenFailedException"/> class.
        /// </summary>
        public EitherGivenOrWhenFailedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EitherGivenOrWhenFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public EitherGivenOrWhenFailedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EitherGivenOrWhenFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public EitherGivenOrWhenFailedException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EitherGivenOrWhenFailedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected EitherGivenOrWhenFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}