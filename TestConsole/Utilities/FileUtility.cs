namespace TestConsole.Utilities
{
    using System;
    using System.IO;
    using System.Linq;

    class FileUtility
    {
        private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
        private static readonly char[] AdditionalInvalidPathChars = new char[] { '*' };

        public static void InsertText(string path, string newText)
        {
            if (File.Exists(path))
            {
                string oldText = File.ReadAllText(path);
                using (var sw = new StreamWriter(path, false))
                {
                    sw.WriteLine(newText);
                    sw.WriteLine(oldText);
                }
            }
        }

        public static void AddAuthorYamlHeaderToMD()
        {
            string authorHeader = "---\nauthor: \"BrucePerlerMS\"\n---\n";
            string[] mdFiles = Directory.GetFiles(@"C:\RMSMigration\RMSRef-pr\Reference\Trans", "*.md", SearchOption.AllDirectories);
            foreach (var mdFile in mdFiles)
            {
                InsertText(mdFile, authorHeader);
            }
        }

        private static void WriteContentToFile(string filePath, string fileContent)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, fileContent);
        }

        public static void ReadFromCSV()
        {
            string[] allLines = File.ReadAllLines(@"C:\Users\ychenu\Desktop\SiteTableEntity.csv");

            var query = (from line in allLines
                         let data = line.Split(',')
                         select new
                         {
                             AllowEmptyArea = data[3],
                             AllowEmptyTheme = data[4],
                             IsSystem = data[5],
                             LiveHostName = data[6].ToString(),
                             LiveUrlTemplate = data[7].ToString(),
                             SiteName = data[8].ToString(),
                             StageHostName = data[9].ToString(),
                             StageUrlTemplate = data[10].ToString()
                         }).ToArray();
            File.WriteAllLines(@"C:\Users\ychenu\Desktop\SiteTable.txt", allLines);
        }

        public static void ValidatePath(string path)
        {
            Guard.ArgumentNotNullOrEmpty(path, nameof(path));

            CheckInvalidCharInPath(path, InvalidPathChars);
            var fileName = Path.GetFileName(path);
            CheckInvalidCharInPath(fileName, InvalidFileNameChars);
            CheckInvalidCharInPath(path, AdditionalInvalidPathChars);
        }

        public static void ValidateRelativePath(string relativePath)
        {
            ValidatePath(relativePath);

            if (Path.IsPathRooted(relativePath))
            {
                throw new ArgumentException($"relativePath: '{relativePath}' can't be a rooted path", relativePath);
            }

            var mockPrefix = GenerateMockPrefix();
            var normalizedPath = Path.GetFullPath(Path.Combine(mockPrefix, relativePath));
            if (!normalizedPath.StartsWith(mockPrefix))
            {
                throw new ArgumentException($"relativePath: '{relativePath}' is above the root path", relativePath);
            }
        }

        public static string NormalizeRelativePath(string relativePath)
        {
            try
            {
                ValidateRelativePath(relativePath);
                var mockPrefix = GenerateMockPrefix();
                var fullPath = Path.GetFullPath(Path.Combine(mockPrefix, relativePath));
                var normalizedRelativePath = fullPath.Length > mockPrefix.Length ? fullPath.Substring(mockPrefix.Length + 1) : string.Empty;
                return normalizedRelativePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
            catch (ArgumentException ae)
            {
                throw new ArgumentException(ae.Message, relativePath, ae);
            }
        }


        #region Private Methods

        private static void CheckInvalidCharInPath(string path, char[] invalidChars)
        {
            var invalidCharIndex = path.IndexOfAny(invalidChars);
            if (invalidCharIndex != -1)
            {
                throw new ArgumentException($"path: '{path}' contains invalid path char: {path[invalidCharIndex]}");
            }
        }

        private static string GenerateMockPrefix() => Path.Combine("c:\\", Guid.NewGuid().ToString("N").Substring(0, 8));

        #endregion
    }
}
