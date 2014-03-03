// <copyright file="Scenario.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>
#if NET40

namespace Xbehave.Internal
{
    using System.Dynamic;

    internal partial class Scenario
    {
        private readonly dynamic context = new ExpandoObject();

        public dynamic Context
        {
            get { return this.context; }
        }
    }
}
#endif
