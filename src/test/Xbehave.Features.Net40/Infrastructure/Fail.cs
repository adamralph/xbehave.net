// <copyright file="Fail.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Features.Infrastructure
{
    public class Fail : Result
    {
        public string Message { get; set; }

        public string ExceptionType { get; set; }
    }
}
