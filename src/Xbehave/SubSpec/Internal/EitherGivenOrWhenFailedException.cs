// <copyright file="EitherGivenOrWhenFailedException.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    // TODO: address code analysis warnings
    [Serializable]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "Part of the original SubSpec code - will be addressed.")]
    [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Justification = "Part of the original SubSpec code - will be addressed.")]
    internal class EitherGivenOrWhenFailedException : Exception
    {
        public EitherGivenOrWhenFailedException(string message)
            : base(message)
        {
        }
    }
}