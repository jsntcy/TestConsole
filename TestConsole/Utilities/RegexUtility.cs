namespace TestConsole.Utilities
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    class RegexUtility
    {
        private static readonly Regex _azureHtmlTitleRegex = new Regex("(\\| Microsoft Azure)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex _azureHtmlIncludeRegex = new Regex(@"^(\<br\s*\/\>)(\s*\r?\n\[AZURE\.INCLUDE)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        //private static readonly Regex _azureHtmlDefinitionRegex = new Regex(@"^ *\[(\[^\]\]+)\]: *\<?(\[^\s\>\]+)\>?(?: +\[""(\](\[^\n\]+)\["")\])? *(?:\n+\|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public static readonly Regex _azureHtmlDefinitionRegex = new Regex(@"^( +)(\[([^\]]+)\]: *<?([^\s>]+)>?(?: +[""(]([^\n]+)["")])? *(?:\r?\n+|$))", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public static readonly Regex _azureHtmlIncludeWithPostfixRegex = new Regex(@"^(\[AZURE\.INCLUDE\s*\[((?:\[[^\]]*\]|[^\[\]]|\](?=[^\[]*\]))*)\]\(\s*<?([^)]*?)>?(?:\s+(['""])([\s\S]*?)\3)?\s*\)\])[\t\f ]*(\S.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private static string ReplaceStr(Match input)
        {
            return input.ToString().TrimStart();
        }

        public static void Replace()
        {
            var title = "as | microsoft azure | microsoft azure";
            var index = title.IndexOf('|');
            if (index != -1)
            {
                title = title.Remove(index).Insert(index, "| Microsoft Docs");
            }
            title = _azureHtmlTitleRegex.Replace(title, match => "| Microsoft Docs");
            var replaced = title.Replace("azure1", "docs");

            var sourceContent1 = File.ReadAllText(@"C:\Users\ychenu\Desktop\migration\connectors-create-api-azureblobstorage_1.md");
            sourceContent1 = _azureHtmlIncludeWithPostfixRegex.Replace(sourceContent1, $"$1{Environment.NewLine}$6");
            sourceContent1 = _azureHtmlIncludeRegex.Replace(sourceContent1, "$2");
            //var matches1 = _azureHtmlIncludeWithPostfixRegex.Matches(sourceContent1);
            sourceContent1 = _azureHtmlDefinitionRegex.Replace(sourceContent1, "$2");
            File.WriteAllText(@"C:\Users\ychenu\Desktop\migration\connectors-create-api-azureblobstorage_1_converted.md", sourceContent1);
            var sourceDirInfo = new DirectoryInfo(@"C:\Git\azure-docs-pr2");
            var fileInfos = sourceDirInfo.GetFiles("*.md", SearchOption.AllDirectories);
            var files = Directory.GetFiles(@"C:\Git\azure-docs-pr2", "*.md", SearchOption.AllDirectories);
            Parallel.ForEach(
                fileInfos,
                new ParallelOptions() { MaxDegreeOfParallelism = 8 },
                fileInfo =>
                {
                    var sourceContent = File.ReadAllText(fileInfo.FullName);
                    sourceContent = _azureHtmlIncludeWithPostfixRegex.Replace(sourceContent, $"$1{Environment.NewLine}$6");
                    //var sourceContent = File.ReadAllText("machine-learning-data-science-process-sqldw-walkthrough.md");
                    sourceContent = _azureHtmlIncludeRegex.Replace(sourceContent, "$2");
                    sourceContent = _azureHtmlDefinitionRegex.Replace(sourceContent, "$2");
                    File.WriteAllText(fileInfo.FullName, sourceContent);
                });
        }
    }
}
