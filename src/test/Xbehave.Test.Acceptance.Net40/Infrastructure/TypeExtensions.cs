// <copyright file="TypeExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

#if NET40 || NET45
namespace Xbehave.Test.Acceptance.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;

    internal static class TypeExtensions
    {
        public static void ClearTestEvents(this Type feature)
        {
            foreach (var path in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*." + feature.Name))
            {
                File.Delete(path);
            }
        }

        public static IEnumerable<string> GetTestEvents(this Type feature)
        {
            return Directory
                .EnumerateFiles(Directory.GetCurrentDirectory(), "*." + feature.Name)
                .Select(fileName => new
                {
                    FileName = fileName,
                    Ticks = long.Parse(File.ReadAllText(fileName), CultureInfo.InvariantCulture),
                })
                .OrderBy(@event => @event.Ticks)
                .Select(@event => Path.GetFileNameWithoutExtension(@event.FileName));
        }

        public static void SaveTestEvent(this Type feature, string @event)
        {
            Thread.Sleep(1);
            using (var file = new StreamWriter(string.Concat(@event, ".", feature.Name), false))
            {
                file.Write(DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}
#endif
