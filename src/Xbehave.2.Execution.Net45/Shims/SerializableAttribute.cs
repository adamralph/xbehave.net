// <copyright file="SerializableAttribute.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if WPA81 || WINDOWS_PHONE
namespace Xbehave.Execution.Shims
{
    using System;

    internal sealed class SerializableAttribute : Attribute
    {
    }
}
#endif
