namespace TestConsole.Utilities
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class ZipUtilities
    {
        public Task ArchiveDirectoryAsync(string directoryToArchive, string archiveFilePath, CompressionLevel compressionLevel, Func<string, bool> fileExcluder, CancellationToken cancellationToken)
        {
            Guard.ArgumentNotNullOrEmpty(directoryToArchive, nameof(directoryToArchive));
            Guard.ArgumentNotNullOrEmpty(archiveFilePath, nameof(archiveFilePath));

            Guard.Argument(() => Directory.Exists(directoryToArchive), nameof(directoryToArchive), $"Directory to archive {directoryToArchive} does not exist.");
            Guard.Argument(() => !File.Exists(archiveFilePath), nameof(archiveFilePath), $"Archive file {archiveFilePath} already exists.");

            return Task.Run(
                () =>
                {
                    if (fileExcluder == null)
                    {
                        ArchiveDirectoryWithoutExcluder(directoryToArchive, archiveFilePath, compressionLevel);
                    }
                    else
                    {
                        ArchiveDirectoryWithExcluder(directoryToArchive, archiveFilePath, compressionLevel, fileExcluder);
                    }
                },
                cancellationToken);
        }

        public Task UnarchiveToDirectoryAsync(string archiveFilePath, string directoryToUnarchiveTo, CancellationToken cancellationToken)
        {
            Guard.ArgumentNotNullOrEmpty(archiveFilePath, nameof(archiveFilePath));
            Guard.ArgumentNotNullOrEmpty(directoryToUnarchiveTo, nameof(directoryToUnarchiveTo));

            Guard.Argument(() => File.Exists(archiveFilePath), nameof(archiveFilePath), $"Archive file {archiveFilePath} does not exist.");
            
            return Task.Run(
                () =>
                {
                    ZipFile.ExtractToDirectory(archiveFilePath, directoryToUnarchiveTo);
                },
                cancellationToken);
        }

        private void ArchiveDirectoryWithoutExcluder(string directoryToArchive, string archiveFilePath, CompressionLevel compressionLevel)
        {
            ZipFile.CreateFromDirectory(directoryToArchive, archiveFilePath, compressionLevel, false);
        }

        private void ArchiveDirectoryWithExcluder(string directoryToArchive, string archiveFilePath, CompressionLevel compressionLevel, Func<string, bool> fileExcluder)
        {
            using (ZipArchive archive = ZipFile.Open(archiveFilePath, ZipArchiveMode.Create))
            {
                var directoryToArchiveFullPath = Path.GetFullPath(directoryToArchive);

                var filesToBeZipped = Directory.EnumerateFiles(directoryToArchive, "*.*", SearchOption.AllDirectories)
                    .Where(file => !fileExcluder(file));

                foreach (var file in filesToBeZipped)
                {
                    var entryName = Path.GetFullPath(file).Substring(directoryToArchiveFullPath.Length + 1);
                    archive.CreateEntryFromFile(file, entryName, compressionLevel);
                }
            }
        }

        public async Task TestZipPerf()
        {
            var watch = Stopwatch.StartNew();
            await ArchiveDirectoryAsync(@"C:\TestFiles\201701240405154586-5140", @"c:\output\output-file.zip", System.IO.Compression.CompressionLevel.Optimal, null, default(CancellationToken));
            watch.Stop();
            Console.WriteLine("output using file: " + watch.ElapsedMilliseconds);

            watch.Restart();
            ArchiveDirectoryWithoutExcluder(@"C:\TestFiles\201701240405154586-5140", @"c:\output\output-directory.zip", System.IO.Compression.CompressionLevel.Optimal);
            watch.Stop();
            Console.WriteLine("output using directory: " + watch.ElapsedMilliseconds);

            watch.Start();
            await ArchiveDirectoryAsync(@"C:\TestFiles\11119cfa-17190aeb-GeneratePdf-cache-201702220232580023-master", @"c:\output\cache-file.zip", System.IO.Compression.CompressionLevel.Optimal, null, default(CancellationToken));
            watch.Stop();
            Console.WriteLine("cache using file: " + watch.ElapsedMilliseconds);

            watch.Start();
            ArchiveDirectoryWithoutExcluder(@"C:\TestFiles\11119cfa-17190aeb-GeneratePdf-cache-201702220232580023-master", @"c:\output\cache-directory.zip", System.IO.Compression.CompressionLevel.Optimal);
            watch.Stop();
            Console.WriteLine("cache using directory: " + watch.ElapsedMilliseconds);
        }
    }
}
