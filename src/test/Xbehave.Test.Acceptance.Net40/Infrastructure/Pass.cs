﻿// <copyright file="Pass.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance.Infrastructure
{
    public class Pass : Result
    {
        public override string ToString()
        {
            return "Passed";
        }
    }
}