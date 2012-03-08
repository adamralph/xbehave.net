// <copyright file="EitherGivenOrWhenFailedException.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;

    [Serializable]
    internal class EitherGivenOrWhenFailedException : Exception
    {
        public EitherGivenOrWhenFailedException(string message)
            : base(message)
        {
        }
    }
}