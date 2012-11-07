// <copyright file="MemberInfoExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.ExpressionNaming
{
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Guard = Xbehave.Sdk.Guard;

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
