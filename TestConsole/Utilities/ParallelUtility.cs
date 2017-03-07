namespace TestConsole.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class SourcePathEqualityComparer : IEqualityComparer<RedirectionEntry>
    {
        public int GetHashCode(RedirectionEntry obj)
        {
            return obj?.SourcePath.GetHashCode() ?? 0;
        }

        public bool Equals(RedirectionEntry x, RedirectionEntry y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return string.Equals(x.SourcePath, y.SourcePath, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class RedirectionEntryUpdateEqualityComparer : IEqualityComparer<RedirectionEntry>
    {
        public int GetHashCode(RedirectionEntry obj)
        {
            return obj?.SourcePath.GetHashCode() ?? 0;
        }

        public bool Equals(RedirectionEntry x, RedirectionEntry y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return string.Equals(x.SourcePath, y.SourcePath, StringComparison.OrdinalIgnoreCase)
                && (!string.Equals(x.RedirectUrl, y.RedirectUrl, StringComparison.OrdinalIgnoreCase)
                || x.RedirectDocumentId != y.RedirectDocumentId);
        }
    }

    class ParallelUtility
    {
        private async static Task<ParallelLoopResult> GenerateRedirectionFiles(IEnumerable<string> entries)
        {
            return await Task.Run(() => Parallel.ForEach(entries, GenerateRedirectionFile));
        }

        private static void GenerateRedirectionFile(string entry, ParallelLoopState state)
        {
            if (!string.IsNullOrEmpty(entry))
            {
                try
                {
                    throw new Exception();
                }
                catch (Exception)
                {
                    state.Break();
                }
            }
        }

        public static Dictionary<string, GitChangeType> ParseRedirectionChangeFiles(IEnumerable<RedirectionEntry> entries, IEnumerable<RedirectionEntry> cachedEntries)
        {
            if (cachedEntries == null && entries == null)
            {
                return null;
            }

            if (cachedEntries == null)
            {
                return entries.ToDictionary(entry => entry.SourcePath, entry => GitChangeType.Created);
            }

            if (entries == null)
            {
                return cachedEntries.ToDictionary(entry => entry.SourcePath, entry => GitChangeType.Deleted);
            }

            var sourcePathEqualityComparer = new SourcePathEqualityComparer();
            var added = entries.Except(cachedEntries, sourcePathEqualityComparer).ToDictionary(entry => entry.SourcePath, entry => GitChangeType.Created);
            var deleted = cachedEntries.Except(entries, sourcePathEqualityComparer).ToDictionary(entry => entry.SourcePath, entry => GitChangeType.Deleted);

            var redirectionEntryUpdateEqualityComparer = new RedirectionEntryUpdateEqualityComparer();
            var updated = entries.Intersect(cachedEntries, redirectionEntryUpdateEqualityComparer).ToDictionary(entry => entry.SourcePath, entry => GitChangeType.Updated);

            return added.Concat(deleted).Concat(updated).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static void Test()
        {
            var mr = JsonUtility.ReadFromJsonFile<MasterRedirection>(@"C:\TestFiles\.redirection.mapping1.json");
            var cachedmr = JsonUtility.ReadFromJsonFile<MasterRedirection>(@"C:\TestFiles\.redirection.mapping.cache1.json");

            var entries = mr?.Entries;
            var cachedEntries = cachedmr?.Entries;

            var gitChangeFiles = ParseRedirectionChangeFiles(entries, cachedEntries);
            //var dup = mr.Entries.GroupBy(entry => entry.SourcePath).Where(group => group.Count() > 1);
            //if (dup.Any())
            //{
            //    foreach (var d in dup)
            //    {
            //        Console.WriteLine(d.Key);
            //    }
            //}
            List<string> files = new List<string>
            {
                "ss",
                ""
            };
            var result = GenerateRedirectionFiles(files).Result;
            if (!Directory.Exists(@"C:\TestFiles\tt\test.txt"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(@"C:\TestFiles\tt\test.txt"));
                File.WriteAllText(@"C:\TestFiles\tt\test.txt", "");
            }
        }
    }
}
