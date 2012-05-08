// <copyright file="IDisposer.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Infra
{
    using System;
    using System.Collections.Generic;

    internal interface IDisposer
    {
        void Dispose(IEnumerable<IDisposable> disposables);
    }
}