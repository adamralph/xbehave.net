using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Xbehave.Execution.Extensions
{
    internal static class MethodInfoExtensions
    {
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "Propagating sync method parameter name.")]
        public static async Task InvokeAsync(this MethodInfo method, object obj, object[] arguments)
        {
            method = method ?? throw new ArgumentNullException(nameof(method));

            var parameterTypes = method.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
            Reflector.ConvertArguments(arguments, parameterTypes);

            if (method.Invoke(obj, arguments) is Task task)
            {
                await task;
            }
        }
    }
}
