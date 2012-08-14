// <copyright file="TypeExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.ExpressionNaming
{
    using System;
    using System.Linq;
    using Guard = Xbehave.Sdk.Guard;

    public static class TypeExtensions
    {
        public static bool AllowsInferenceOf(this Type type, Type genericType)
        {
            Guard.AgainstNullArgument("type", type);
            Guard.AgainstNullArgument("genericType", genericType);

            return type.Name == genericType.Name ||
                (type.IsGenericType &&
                    type.GetGenericArguments().Aggregate(false, (current, genericArg) => current || genericArg.AllowsInferenceOf(genericType)));
        }
    }
}
