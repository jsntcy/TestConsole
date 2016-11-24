namespace TestConsole
{
    using System.Text.RegularExpressions;

    public static class NameConstraints
    {
        public const string DocsetNamePattern = @"^[^\?\&\s\/\\]+$";
        public const string ProductNamePattern = @"^[^\?\&\s\/\\\.]+$";
        public const string SiteNamePattern = @"^[^\?\&\s\\]+$";
        public const string RepositoryNamePattern = @"^[A-Za-z0-9_.-]+$";

        public static readonly Regex DocsetNameRegex = new Regex(DocsetNamePattern, RegexOptions.Compiled);
        public static readonly Regex ProductNameRegex = new Regex(ProductNamePattern, RegexOptions.Compiled);
        public static readonly Regex SiteNameRegex = new Regex(SiteNamePattern, RegexOptions.Compiled);
        public static readonly Regex RepositoryNameRegex = new Regex(RepositoryNamePattern, RegexOptions.Compiled);
    }
}
