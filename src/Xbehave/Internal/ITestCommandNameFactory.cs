// <copyright file="ITestCommandNameFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    internal interface ITestCommandNameFactory
    {
        string Create(Step given, Step when, Step then);

        string CreateSetup(Step given, Step when);

        string CreateTeardown(Step given, Step when);
    }
}
