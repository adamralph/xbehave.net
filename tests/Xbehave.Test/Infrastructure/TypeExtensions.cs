namespace Xbehave.Test.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;

    internal static class TypeExtensions
    {
        private static int eventIndex = 0;
        private static readonly SemaphoreSlim fileSystem = new SemaphoreSlim(1, 1);

        public static void ClearTestEvents(this Type feature)
        {
            fileSystem.Wait();
            try
            {
                foreach (var path in Directory.EnumerateFiles(feature.GetDirectoryName(), "*." + feature.Name))
                {
                    File.Delete(path);
                }
            }
            finally
            {
                fileSystem.Release();
            }
        }

        public static IEnumerable<string> GetTestEvents(this Type feature)
        {
            fileSystem.Wait();
            try
            {
                return Directory
                    .EnumerateFiles(feature.GetDirectoryName(), "*." + feature.Name)
                    .Select(fileName => new
                    {
                        FileName = fileName,
                        Index = int.Parse(File.ReadAllText(fileName), CultureInfo.InvariantCulture),
                    })
                    .OrderBy(@event => @event.Index)
                    .Select(@event => Path.GetFileNameWithoutExtension(@event.FileName)).ToArray();
            }
            finally
            {
                fileSystem.Release();
            }
        }

        public static void SaveTestEvent(this Type feature, string @event)
        {
            fileSystem.Wait();
            try
            {
                using (var file = File.Create(Path.Combine(feature.GetDirectoryName(), string.Concat(@event, ".", feature.Name))))
                using (var writer = new StreamWriter(file))
                {
                    writer.Write(eventIndex++.ToString(CultureInfo.InvariantCulture));
                }
            }
            finally
            {
                fileSystem.Release();
            }
        }

        private static string GetDirectoryName(this Type feature) =>
            Path.GetDirectoryName(feature.Assembly.GetFileName());
    }
}
