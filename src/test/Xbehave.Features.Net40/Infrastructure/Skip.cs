// <copyright file="Skip.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Features.Infrastructure
{
    public class Skip : Result
    {
        public string Reason { get; set; }

        public override string ToString()
        {
            return string.Concat("Skipped: ", this.Reason);
        }
    }
}
