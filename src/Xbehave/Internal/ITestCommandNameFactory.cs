// <copyright file="ITestCommandNameFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    internal interface ITestCommandNameFactory
    {
        string CreateSharedContext(Step given, Step when);

        string CreateSharedStep(Step given, Step when, Step then);

        string CreateDisposal(Step given, Step when);

        string CreateIsolatedStep(Step given, Step when, Step then);

        string CreateSkippedStep(Step given, Step when, Step then);
    }
}
