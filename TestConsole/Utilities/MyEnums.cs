using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole.Utilities
{
    public static class GitChangeTypeExtensions
    {
        public static string GetGitChangeTypeFullName(this GitChangeType blobPackageType)
        {
            return $"{nameof(GitChangeType)}.{blobPackageType}";
        }
    }
}
