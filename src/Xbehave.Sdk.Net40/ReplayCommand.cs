﻿// <copyright file="ReplayCommand.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;
    using Xunit.Sdk;

    public class ReplayCommand : TestCommand
    {
        private readonly MethodResult result;

        public ReplayCommand(IMethodInfo method, MethodResult result)
            : base(method, null, 0)
        {
            this.result = result;
        }

        public override bool ShouldCreateInstance
        {
            get { return false; }
        }

        public override MethodResult Execute(object testClass)
        {
            return this.result;
        }
    }
}