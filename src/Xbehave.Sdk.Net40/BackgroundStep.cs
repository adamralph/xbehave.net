// <copyright file="BackgroundStep.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk
{
    using System;

    public class BackgroundStep : Step
    {
        public BackgroundStep(string stepType, string description, Action body)
            : base(stepType, description, body)
        {
        }

        public override string Name
        {
            get { return "(Background) " + base.Name; }
        }
    }
}