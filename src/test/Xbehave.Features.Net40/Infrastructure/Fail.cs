// <copyright file="Fail.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Features.Infrastructure
{
    using System;

    public class Fail : Result
    {
        public string Message { get; set; }

        public string ExceptionType { get; set; }

        public string StackTrace { get; set; }

        public override string ToString()
        {
            return string.Concat(
                "Failed: ",
                this.ExceptionType,
                ": ",
                this.Message,
                Environment.NewLine,
                this.StackTrace);
        }
    }
}
