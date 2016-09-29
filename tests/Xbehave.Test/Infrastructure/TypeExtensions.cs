// <copyright file="TypeExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    internal static class TypeExtensions
    {
        public static void ClearTestEvents(this Type feature)
        {
            foreach (var path in Directory
                .EnumerateFiles(GetDirectoryName(feature), "*." + feature.Name))
            {
                File.Delete(path);
            }
        }

        public static IEnumerable<string> GetTestEvents(this Type feature)
        {
            return Directory
                .EnumerateFiles(GetDirectoryName(feature), "*." + feature.Name)
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
            using (var file = File.Create(
                Path.Combine(GetDirectoryName(feature), string.Concat(@event, ".", feature.Name))))
            using (var writer = new StreamWriter(file))
            {
                writer.Write(DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture));
            }
        }

        private static string GetDirectoryName(Type feature)
        {
            return Path.GetDirectoryName(new Uri(feature.GetTypeInfo().Assembly.CodeBase).LocalPath);
        }
    }
}
