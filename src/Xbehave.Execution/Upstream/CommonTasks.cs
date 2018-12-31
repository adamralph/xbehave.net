// UPSTREAM: https://raw.githubusercontent.com/xunit/xunit/2.4.1/src/common/CommonTasks.cs
#if !NET35

using System.Threading.Tasks;

static class CommonTasks
{
    internal static readonly Task Completed = Task.FromResult(0);
}

#endif