﻿namespace TestConsole.Utilities
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public static class FolderUtility
    {
        public static bool IsEmpty(string directory)
        {
            Guard.ArgumentNotNullOrEmpty(directory, nameof(directory));
            Guard.Argument(() => Directory.Exists(directory), nameof(directory), $"Directory '{directory}' does not exist");

            return !Directory.EnumerateFileSystemEntries(directory).Any();
        }

        public static bool ExistsAndIsNotEmpty(string directory)
        {
            Guard.ArgumentNotNullOrEmpty(directory, nameof(directory));

            return Directory.Exists(directory) && !IsEmpty(directory);
        }

        public static void CopyDirectoryWithAllSubDirectories(string sourceDirectory, string targetDirectory, int maxDegreeOfParallelism = -1)
        {
            Guard.ArgumentNotNullOrEmpty(sourceDirectory, nameof(sourceDirectory));
            Guard.ArgumentNotNullOrEmpty(targetDirectory, nameof(targetDirectory));
            Guard.Argument(() => Directory.Exists(sourceDirectory), nameof(sourceDirectory), $"source directory '{sourceDirectory}' does not exist");

            Directory.CreateDirectory(targetDirectory);
            foreach (var directory in Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(directory.Replace(sourceDirectory, targetDirectory));
            }

            Parallel.ForEach(
                Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories),
                new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism },
                file => File.Copy(file, file.Replace(sourceDirectory, targetDirectory), true));
        }

        public static void ForceDeleteDirectoryWithAllSubDirectories(string directory, int maxDegreeOfParallelism = -1)
        {
            Guard.ArgumentNotNullOrEmpty(directory, nameof(directory));

            if (Directory.Exists(directory))
            {
                ForceDeleteAllSubDirectories(directory, maxDegreeOfParallelism);
                Directory.Delete(directory, true);
            }
        }

        public static void ForceDeleteAllSubDirectories(string directory, int maxDegreeOfParallelism = -1)
        {
            Guard.ArgumentNotNullOrEmpty(directory, nameof(directory));

            if (Directory.Exists(directory))
            {
                var dirs = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
                Parallel.ForEach(
                    Directory.GetFiles(directory, "*", SearchOption.AllDirectories),
                    new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism },
                    ForceDeleteFile);
            }
        }

        public static void ForceDeleteFile(string filePath)
        {
            //throw new Exception();
            Guard.ArgumentNotNullOrEmpty(filePath, nameof(filePath));

            File.SetAttributes(filePath, FileAttributes.Normal);
            File.Delete(filePath);
        }

        public static void DeleteFileInBackground(string filePath)
        {
            Guard.ArgumentNotNullOrEmpty(filePath, nameof(filePath));

            TaskHelper.RunTaskSilentlyNoWait(Task.Run(() => File.Delete(filePath)), "Delete file in background.");
        }

        public static async void DeleteDirectoryInBackground(string directory, int maxDegreeOfParallelism = -1)
        {
            Guard.ArgumentNotNullOrEmpty(directory, nameof(directory));

            await Task.Run(() => ForceDeleteDirectoryWithAllSubDirectories(directory, maxDegreeOfParallelism));
        }
    }
}
