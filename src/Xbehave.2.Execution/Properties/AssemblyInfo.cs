// <copyright file="AssemblyInfo.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

using System.Reflection;
using Xunit.Sdk;

#if PLATFORM_DOTNET
[assembly: AssemblyTitle("xBehave.net Execution (dotnet)")]
#else 
[assembly: AssemblyTitle("xBehave.net Execution (desktop)")]
#endif
[assembly: AssemblyDescription("The xBehave.net execution library.")]
[assembly: PlatformSpecificAssembly]
