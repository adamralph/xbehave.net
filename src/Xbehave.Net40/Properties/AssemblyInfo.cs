// <copyright file="AssemblyInfo.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

[assembly: AssemblyTitle("xBehave.net")]
[assembly: AssemblyDescription(
    "A fluent and concise BDD/TDD library based on xUnit.net designed for use either from day one or as a seamless addition to an existing xUnit.net based workflow.")]

[assembly: SuppressMessage(
    "Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Xbehave.Fluent", Justification = "Not designed for explicit referencing")]