// <copyright file="Scenario.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
#if NET40
    using System.Dynamic;
#endif

    internal partial class Scenario
    {
#if NET40
        private readonly dynamic context = new ExpandoObject();

        public dynamic Context
        {
            get { return this.context; }
        }
#endif
    }
}