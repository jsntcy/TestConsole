namespace TestConsole.Utilities
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a simple class for validating parameters and throwing exceptions.
    /// </summary>
    public static class UpdatePackageVersion
    {
        public static async Task Update(string directory, string[] extensions, string oldVersion, string newVersion)
        {
            Guard.ArgumentNotNullOrEmpty(directory, nameof(directory));
            Guard.ArgumentNotNull(extensions, nameof(extensions));
            Guard.ArgumentNotNullOrEmpty(oldVersion, nameof(oldVersion));
            Guard.ArgumentNotNullOrEmpty(newVersion, nameof(newVersion));

            foreach (var extension in extensions)
            {
                var files = Directory.GetFiles(directory, "*" + extension, SearchOption.AllDirectories);
                    //.Where(path => extensions.Contains(Path.GetExtension(path))).ToList();
                Parallel.ForEach(
                    files,
                    new ParallelOptions { MaxDegreeOfParallelism = 8 },
                    file => UpdateCore(file, oldVersion, newVersion));
            }
        }

        private static void UpdateCore(string filePath, string oldVersion, string newVersion)
        {
            var content = File.ReadAllText(filePath);
            if (content.Contains(oldVersion))
            {
                content = content.Replace(oldVersion, newVersion);
                File.WriteAllText(filePath, content);
            }
        }
    }
}
