using System;
using System.Reflection;
using System.Linq.Expressions;

namespace TestUtility
{
    internal static class StaticReflection
    {
        public static FieldInfo FieldOf<T>( Expression<System.Func<T>> expression )
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return (FieldInfo)body.Member;
        }

        public static MethodInfo MethodOf<T>( Expression<System.Func<T>> expression )
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }

        public static MethodInfo MethodOf( Expression<System.Action> expression )
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }
    }
}
