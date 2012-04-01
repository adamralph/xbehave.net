using System;
using System.Reflection;
using System.Linq.Expressions;

namespace TestUtility
{
    internal static class StaticReflection
    {
        public static MethodInfo MethodOf( Expression<System.Action> expression )
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
    }
}
