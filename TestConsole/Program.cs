using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestConsole.Models;
using TestConsole.Utilities;
using static TestConsole.Utilities.PerformanceScopeUtility;

using MsExchange = Microsoft.Exchange.WebServices.Data;

namespace TestConsole
{
    public class Work
    {
        public string WorkingDirectory { get; }

        public string SourceDirectory => Path.Combine(WorkingDirectory, "source");

        public Work()
        {
            WorkingDirectory = Path.Combine("w:", Path.GetRandomFileName().Substring(0, 4));
            Console.WriteLine(SourceDirectory);
        }
    }

    public enum BlobPackageType
    {
        Source,
        Output,
        Cache,
        CacheSnapshot,
        Log
    }

    public class Base
    {
        public string _s { get; set; }
        public Base()
        {
            _s = "base";
        }
    }

    public class PackageDownloadAndUnzipPathInfo : Base
    {
        public PackageDownloadAndUnzipPathInfo()
            : this(null)
        {
            Console.WriteLine("PackageDownloadAndUnzipPathInfo");
        }

        public PackageDownloadAndUnzipPathInfo(string s)
        {
            s = s ?? "abl";
            Console.WriteLine("PackageDownloadAndUnzipPathInfo " + s);
        }
        public string PackageBlobDownloadPath { get; set; }

        public string DestinationDirectory { get; set; }
    }

    public class DerivedPackageDownloadAndUnzipPathInfo : PackageDownloadAndUnzipPathInfo
    {
        public DerivedPackageDownloadAndUnzipPathInfo()
            : this(null)
        {
            Console.WriteLine("DerivedPackageDownloadAndUnzipPathInfo");
        }

        public DerivedPackageDownloadAndUnzipPathInfo(string s)
            : base(s)
        {
            Console.WriteLine("DerivedPackageDownloadAndUnzipPathInfo " + _s);
        }
    }

    public class Item
    {
        public string Version { get; set; }
        public string Url { get; set; }
    }

    class Program
    {
        public async static Task DoStuff()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(5000);
            });
            throw new Exception("do stufff");
            Console.WriteLine("sleep done: {0}", 8);
        }

        private static async Task aggre()
        {
            await Task.WhenAll(Task.Delay(2000), DoStuff());
        }


        public static void CreateOrUpdateDocument(string assetId, int branchName, CreateOrUpdateDocumentRequest request)
        {
            var customPair = new Dictionary<string, object>
            {
                { nameof(assetId), assetId },
                { nameof(request), request }
            };
        }

        public class CreateOrUpdateDocumentRequest
        {
            public string ContentSourceUri
            {
                get;
                set;
            }

            public Dictionary<string, object> Metadata
            {
                get;
                set;
            }
        }

        [Flags]
        public enum OpsPermission
        {
            /// <summary>
            /// Doesn't require any permission
            /// </summary>
            None = 0,

            ReadOnly = 1,

            MonikerAdmin = 1 << 1,
        }

        static void testsb(StringBuilder sb)
        {
             sb.AppendLine("into testsb");
        }

        static void Main(string[] args)
        {
            try
            {
                using (var reader = new StreamReader(@"C:\TestFiles\log.json"))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var logItem = JsonUtility.FromJsonString<LogItem>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            var directory = @"C:\Git\OpenPublishing.Build";
            var extensions = new string[] { ".config", ".csproj" };
            var oldVersion = "2.19.0-alpha-0681-g405ed2d";
            var newVersion = "2.19.0";
            UpdatePackageVersion.Update(directory, extensions, oldVersion, newVersion);
            Convert.ToBoolean("s");
            bool result;
            if (bool.TryParse(null, out result))
            {
                var ssss = result;
            }
            var sbtest = new StringBuilder();
            testsb(sbtest);
            sbtest.AppendLine("out of testsb");
            Console.WriteLine(sbtest.ToString());
            var silent = false;
            try
            {
                throw new FileNotFoundException("");
            }
            catch (Exception) when (silent)
            {
                Console.WriteLine("catch");
            }

            var li = new List<int> {1, 2};
            var second = new List<int> { 3, 2 };
            var exc = li.Except(second);
            li.Add(1);
            li.Add(1);
            var permission = OpsPermission.ReadOnly;
            permission |= OpsPermission.MonikerAdmin;
            var re = new int[] { 1 }.Where(x => x == 3);
            var co = re.Count();
            CancellationTokenSource cts = new CancellationTokenSource();
            ParallelOptions po = new ParallelOptions();
            po.CancellationToken = cts.Token;

            Task.Factory.StartNew(() =>
            {
                if (Console.ReadKey().KeyChar == 'c')
                    cts.Cancel();
                Console.WriteLine("press any key to exit");
            });

            Parallel.ForEach(new List<int>(), po, (algo) =>
            {
                Task.Delay(2000).Wait(); // this compute lasts 1 minute  
                Console.WriteLine("this job is finished");
                po.CancellationToken.ThrowIfCancellationRequested();
            });

            try
            {
                Task.Run(() =>
                {
                    for (var i = 0; i < 100; ++i)
                    {
                        throw new Exception("throw from run");
                    }
                });
            }
            catch (AggregateException aex)
            {
                Console.WriteLine("aex");
            }
            catch(Exception ex)
            {
                Console.WriteLine("ex");
            }

            var exchangeSvc = new MsExchange.ExchangeService(MsExchange.ExchangeVersion.Exchange2010)
            {
                Url = new Uri("https://outlook.office365.com/ews/exchange.asmx"),
                Credentials = new MsExchange.WebCredentials("vscopbld@microsoft.com", "#Bugsfor$-160802"),
                TraceEnabled = true
            };

            var message = new MsExchange.EmailMessage(exchangeSvc);

            message.ToRecipients.Add("ychenu@microsoft.com");

            message.Subject = "test";
            message.Body = "test body";

            message.Save();
            message.Send();

            CreateOrUpdateDocument("abc_id", 6,
                new CreateOrUpdateDocumentRequest
                {
                    ContentSourceUri = null,
                    Metadata = new Dictionary<string, object>
                    {
                        { "assetId", "s" },
                        { "d", 7}
                    }
                });
            var name = $"{nameof(args)}.{nameof(args.Rank)}";
            var ar = new int[] { 6, 7, 3 };
            var sortAr = ar.OrderByDescending(x => x);
            try
            {
                var fo = $"{"a"}{null}";
                var items = new List<Item>
                {
                    new Item { Version = "v1", Url = "d" },
                    new Item { Version = "v1", Url = "b" },
                    new Item { Version = "v1", Url = "c" },
                    new Item { Version = "v2", Url = "f" },
                    new Item { Version = "v2", Url = "a" }
                };

                var web = new HtmlWeb()
                {
                    //PreRequest = request =>
                    //{
                    //    // Make any changes to the request object that will be used.
                    //    request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    //    return true;
                    //}
                };
                var document = web.Load(@"https://opdhsblobsandbox01.blob.core.windows.net/contents/0ced3babc6274b949a9696c03ed9d944/3094b7719e20479798997b70294c9ee3");
                FolderUtility.ForceDeleteAllSubDirectories(@"C:\TestFiles\Test", 1);
            }
            catch (AggregateException aex)
            {
                Console.WriteLine("aex");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ex");
            }
            var array = new Task[] { };
            var f = array.FirstOrDefault();
            Array.ForEach(array, ele =>
            {
                Console.WriteLine(ele);
            });
            var commentSb = new StringBuilder();
            for (int j = 0; j < 2; j++)
            {
                commentSb.AppendFormat(" [View]({0})", "path" + j);
                commentSb.Append("<br/>");
            }

            commentSb.Length -= "<br/>".Length;

            commentSb.AppendLine(@"
File | Status | Preview URL | Details
---- | ------ | ----------- | -------");

            for (int i = 0; i < 2; i++)
            {
                commentSb.AppendFormat(
                    "[{0}]({1}) | {2} |",
                    "path" + i,
                    "http://abc" + i,
                    "success");

                var sb = new StringBuilder();
                for (int j = 0; j < 2; j++)
                {
                    commentSb.AppendFormat(" [View]({0})", "path" + j);
                    if (j == 0)
                    {
                        commentSb.AppendFormat("({0})", j);
                    }
                    commentSb.Append("<br/>");
                }

                commentSb.AppendFormat(" |");

                commentSb.AppendFormat(" [Details](#user-content-{0})", "details");

                commentSb.AppendLine();
            }

                var strsb = commentSb.ToString();
                File.WriteAllText(@"C:\TestFiles\comment.md", strsb);

                //var derived = new DerivedPackageDownloadAndUnzipPathInfo();
                string source = null;
            Console.WriteLine($"{source}-abc");
            var packageTypeToPathsMap = new Dictionary<BlobPackageType, PackageDownloadAndUnzipPathInfo>();

            var skipPackageDownloadingMap = new Dictionary<BlobPackageType, bool>();
            skipPackageDownloadingMap[BlobPackageType.Source] = true;
            skipPackageDownloadingMap[BlobPackageType.Cache] = true;
            var skip = JsonUtility.ToJsonString(skipPackageDownloadingMap);
            var packageTypeToSkipFlagMap = JsonUtility.FromJsonString<Dictionary<BlobPackageType, bool>>(skip);
            foreach (var packageTypeToSkipFlagKeyValuePair in packageTypeToSkipFlagMap)
            {
                var blobPackageType = packageTypeToSkipFlagKeyValuePair.Key;
                var skipPackageDownloading = packageTypeToSkipFlagKeyValuePair.Value;
                //if (!skipPackageDownloading)
                {
                    var packageBlobDownloadPath = packageTypeToPathsMap[blobPackageType].PackageBlobDownloadPath;

                }
            }
            var path = Path.GetTempPath();
            try
            {
                var details = "data ex";
                throw new InvalidDataException($"invalid {details}");
            }
            catch (InvalidDataException idex)
            {
                Console.WriteLine($"data ex: {idex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ex");
            }
            var workingDirectory = Path.Combine(@"c:\users\ychenu", Path.GetRandomFileName().Substring(0, 4));
            var work = new Work();
        }
    }
}
