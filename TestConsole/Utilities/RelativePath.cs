namespace TestConsole.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// relative path
    /// </summary>
    public class RelativePath
    {
        #region Consts/Fields

        private static readonly char[] InvalidChars = Path.GetInvalidPathChars().Concat(":").ToArray();
        private static readonly RelativePath Empty = new RelativePath(false, 0, new string[] { string.Empty });
        private static readonly RelativePath WorkingFolder = new RelativePath(true, 0, new string[] { string.Empty });

        private const string NormalizedWorkingFolder = "~/";
        private const string ParentDirectory = "../";
        private readonly bool _isFromWorkingFolder;
        private readonly int _parentDirectoryCount;
        private readonly string[] _parts;

        #endregion

        #region Constructor

        private RelativePath(bool isFromWorkingFolder, int parentDirectoryCount, string[] parts)
        {
            _isFromWorkingFolder = isFromWorkingFolder;
            _parentDirectoryCount = parentDirectoryCount;
            _parts = parts;
        }

        #endregion

        #region Public Members

        public static string Normalize(string path)
        {
            return (RelativePath)path;
        }

        public override string ToString() =>
            (_isFromWorkingFolder ? NormalizedWorkingFolder : "") +
            string.Concat(Enumerable.Repeat(ParentDirectory, _parentDirectoryCount)) +
            string.Join("/", _parts);

        #endregion

        #region Private Members

        private static RelativePath Parse(string path) => TryParseCore(path, true);

        private static RelativePath TryParse(string path) => TryParseCore(path, false);

        private static RelativePath TryParseCore(string path, bool throwOnError)
        {
            if (path == null)
            {
                if (throwOnError)
                {
                    throw new ArgumentNullException(nameof(path));
                }
                return null;
            }
            if (path.Length == 0)
            {
                return Empty;
            }
            if (path.IndexOfAny(InvalidChars) != -1)
            {
                if (throwOnError)
                {
                    throw new ArgumentException($"Path({path}) contains invalid char.", nameof(path));
                }
                return null;
            }
            if (Path.IsPathRooted(path))
            {
                if (throwOnError)
                {
                    throw new ArgumentException($"Rooted path({path}) is not supported", nameof(path));
                }
                return null;
            }
            var isFromWorkingFolder = false;
            var parts = path.Replace('\\', '/').Split('/');
            var stack = new Stack<string>();
            var parentCount = 0;
            foreach (var part in parts)
            {
                switch (part)
                {
                    case "~":
                        if (parentCount > 0 || stack.Count > 0 || isFromWorkingFolder)
                        {
                            throw new InvalidOperationException($"Invalid path: {path}");
                        }
                        isFromWorkingFolder = true;
                        break;
                    case "..":
                        if (stack.Count > 0)
                        {
                            stack.Pop();
                        }
                        else
                        {
                            parentCount++;
                        }
                        break;
                    case ".":
                    case "":
                        break;
                    default:
                        stack.Push(part);
                        break;
                }
            }
            if (parts[parts.Length - 1].Length == 0)
            {
                // if end with "/", treat it as folder
                stack.Push(string.Empty);
            }
            return Create(isFromWorkingFolder, parentCount, stack.Reverse());
        }

        private static RelativePath Create(bool isFromWorkingFolder, int parentDirectoryCount, IEnumerable<string> parts)
        {
            var partArray = parts.ToArray();
            if (parentDirectoryCount == 0 &&
                (partArray.Length == 0 ||
                 (partArray.Length == 1 &&
                  partArray[0].Length == 0)))
            {
                return isFromWorkingFolder ? WorkingFolder : Empty;
            }
            return new RelativePath(isFromWorkingFolder, parentDirectoryCount, partArray);
        }

        #endregion

        #region Operators

        public static implicit operator string(RelativePath path)
        {
            return path?.ToString();
        }

        public static explicit operator RelativePath(string path)
        {
            return path == null ? null : Parse(path);
        }

        #endregion
    }
}
