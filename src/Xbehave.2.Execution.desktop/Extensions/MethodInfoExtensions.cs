﻿// <copyright file="MethodInfoExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution.Extensions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Xunit.Sdk;

    internal static class MethodInfoExtensions
    {
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "Propagating sync method parameter name.")]
        public static async Task InvokeAsync(this MethodInfo method, object obj, object[] arguments)
        {
            Guard.AgainstNullArgument("method", method);

            var parameterTypes = method.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
            Reflector.ConvertArguments(arguments, parameterTypes);

            var result = method.Invoke(obj, arguments);
            var task = result as Task;
            if (task != null)
            {
                await task;
            }
        }
    }
}
