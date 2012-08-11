// <copyright file="ActionExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Sdk.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public static class ActionExtensions
    {
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Emulating nested using blocks.")]
        public static void InvokeAll(this IEnumerable<Action> actions)
        {
            if (actions != null)
            {
                Exception lastEx = null;
                foreach (var action in actions.Where(action => action != null))
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception ex)
                    {
                        lastEx = ex;
                    }
                }

                if (lastEx != null)
                {
                    Xunit.Sdk.ExceptionUtility.RethrowWithNoStackTraceLoss(lastEx);
                }
            }
        }
    }
}
