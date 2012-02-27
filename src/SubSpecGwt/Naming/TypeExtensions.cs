// <copyright file="TypeExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace SubSpec.Naming
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal static class TypeExtensions
    {
        public static bool AllowsInferenceOf(this Type type, Type genericType)
        {
            return type.Name == genericType.Name ||
                (type.IsGenericType &&
                    type.GetGenericArguments().Aggregate(false, (current, genericArg) => current || genericArg.AllowsInferenceOf(genericType)));
        }

        public static bool IsCompilerGenerated(this Type type)
        {
            return type.IsDefined(typeof(CompilerGeneratedAttribute), false);
        }
    }
}
