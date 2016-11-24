namespace TestConsole.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;

    class MarkdownUtility
    {
        public static void GenerateMD()
        {
            var buildStatus = ":white_check_mark:Succeeded with warnings, no new warnings";
            var publishUrl = "https://review.docs.microsoft.com/en-us/azure/architecture-overview?branch=pr-en-us-66";
            var commentSb = new StringBuilder();
            commentSb.AppendLine("# Open Publishing Service");
            commentSb.AppendLine("Your pull request has been validated and published.");
            commentSb.AppendLine();
            commentSb.AppendLine($"### Validation status: {buildStatus}");
            commentSb.AppendLine();
            commentSb.AppendLine("Here is the list of files that get changed in this pull request:");
            commentSb.AppendLine();
            commentSb.AppendLine(@"
            File | Status | Preview URL | Details
            ---- | --------| --------| ------------");

            for (int i = 0; i < 2; i++)
            {
                var file = "articles/architecture-overview.md";
                var buildFileLink = "https://github.com/zhenjiao-ms/azure-docs-pr-1/blob/b9c8d92ba8880d4a255acbe6f279b7b628a7f263/articles/architecture-overview.md";
                var buildFileStatus = ":x:Error";

                commentSb.AppendLine($"[{file}]({buildFileLink}) | {buildFileStatus} | [View]({publishUrl}) | [Details](#user-content-articles/architecture-overview.md6)");
            }

            commentSb.AppendLine();

            commentSb.AppendLine("Here are detailed warnings/errors in each file:");
            for (int i = 0; i < 7; i++)
            {
                var file = "articles/architecture-overview.md";

                if (true)
                {
                    var file1 = "articles/architecture-overview.md" + i.ToString();
                    commentSb.AppendLine($"<a name=\"{file1}\"></a>");
                    //commentSb.AppendLine("<a name="{file}"></a>");
                    var buildFileLink = "https://github.com/zhenjiao-ms/azure-docs-pr-1/blob/b9c8d92ba8880d4a255acbe6f279b7b628a7f263/articles/architecture-overview.md";
                    commentSb.AppendLine($"## [{file}]({buildFileLink})");

                    for (int j = 0; j < 2; j++)
                    {
                        var line = "27";
                        line = string.IsNullOrEmpty(line) ? "Unspecified" : line;
                        var message = "articles/architecture-overview.md contains illegal link: #Drawing-symbol-and-icon-sets. The file articles/architecture-overview.md doesn't contain a bookmark named Drawing-symbol-and-icon-sets.";
                        commentSb.AppendLine($" - **Line {line}**: **[Error]** {HttpUtility.HtmlEncode(message)}");
                    }

                    for (int j = 0; j < 2; j++)
                    {
                        var line = string.Empty;
                        line = string.IsNullOrEmpty(line) ? "Unspecified" : line;
                        var message = "articles/architecture-overview.md contains illegal link: #Drawing-symbol-and-icon-sets. The file articles/architecture-overview.md doesn't contain a bookmark named Drawing-symbol-and-icon-sets.";
                        commentSb.AppendLine($" - **Line {line}**: **[Warning]** {HttpUtility.HtmlEncode(message)}");
                    }
                }

                commentSb.AppendLine();
            }

            int truncatedUrlNumber = 8;
            if (truncatedUrlNumber > 0)
            {
                commentSb.AppendLine($"{truncatedUrlNumber} more files are omitted as there're too many files changed in the pull request.");
            }

            string strSB = commentSb.ToString();

            List<string> ls = new List<string>() { "abc", "sd" };
            var tk = ls.Take(1).ToArray();
            var find = ls.FirstOrDefault(x => x == "abc");
            var arr = new string[] { };
            var myDic1 = new Dictionary<string, object>();
            myDic1.Add("a", null);
            object oTest;
            if (myDic1.TryGetValue("a", out oTest))
            {
                string sss = oTest as string;
                Console.WriteLine("true");
            }
        }
    }
}
