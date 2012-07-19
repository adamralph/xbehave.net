// <copyright file="MemberInfoExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.ExpressionNaming
{
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Xbehave.Sdk.Infrastructure;

    public static class MemberInfoExtensions
    {
        public static bool IsExtension(this MemberInfo member)
        {
            Guard.AgainstNullArgument("member", member);
            
            return member.IsDefined(typeof(ExtensionAttribute), false);
        }

        public static bool IsCompilerGenerated(this MemberInfo member)
        {
            Guard.AgainstNullArgument("member", member);

            return member.IsDefined(typeof(CompilerGeneratedAttribute), false);
        }
    }
}
