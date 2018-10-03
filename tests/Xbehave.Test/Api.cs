// <copyright file="Api.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if NETCOREAPP2_0
namespace Xbehave.Test
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using ApprovalTests;
    using ApprovalTests.Namers.StackTraceParsers;
    using ApprovalTests.Reporters;
    using ApprovalTests.StackTraceParsers;
    using PublicApiGenerator;
    using Xbehave;
    using Xunit;
    using Xunit.Sdk;

    public class Api
    {
        static Api() => StackTraceParser.AddParser(new WindowsFactStackTraceParser());

        [WindowsFact]
        [UseReporter(typeof(QuietReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void IsUnchanged() =>
            Approvals.Verify(ApiGenerator.GeneratePublicApi(typeof(ScenarioAttribute).Assembly));

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        [XunitTestCaseDiscoverer("Xunit.Sdk.FactDiscoverer", "xunit.execution.{Platform}")]
        private class WindowsFactAttribute : FactAttribute
        {
            public WindowsFactAttribute()
                : base() =>
                this.Skip = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? null : "Only works on Windows.";
        }

        private class WindowsFactStackTraceParser : XUnitStackTraceParser
        {
            protected override string GetAttributeType() => "Xbehave.Test.Api+WindowsFactAttribute";
        }
    }
}
#endif
