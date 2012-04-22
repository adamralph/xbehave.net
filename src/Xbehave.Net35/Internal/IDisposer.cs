// <copyright file="IDisposer.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;

    internal interface IDisposer
    {
        void Dispose(Stack<IDisposable> disposables);
    }
}