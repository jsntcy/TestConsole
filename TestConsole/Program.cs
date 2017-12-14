using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TestConsole.Models;
using TestConsole.Utilities;
using Newtonsoft.Json;
using static TestConsole.Utilities.PerformanceScopeUtility;

using MsExchange = Microsoft.Exchange.WebServices.Data;

namespace TestConsole
{
    public class MyLogger
    {
        public MyLogger()
        {
            Console.WriteLine("MyLogger constructor");
        }
    }

    public class DefaultConsoleApplicationLogger
    {
        private static readonly MyLogger Logger;

        static DefaultConsoleApplicationLogger()
        {
            Console.WriteLine("DefaultConsoleApplicationLogger constructor");
            Logger = new MyLogger();
        }

        public static MyLogger GetLogger()
        {
            return Logger;
        }
    }

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

    public class Hash
    {
        private readonly Dictionary<string, object> _nestedDictionary;

        public Hash()
        {
            this._nestedDictionary = new Dictionary<string, object>();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            ((ICollection<KeyValuePair<string, object>>)this._nestedDictionary).Add(item);
        }

        public void Add(string key, object value)
        {
            this._nestedDictionary.Add(key, value);
        }

        public static Hash FromDictionary(IDictionary<string, object> dictionary)
        {
            Hash hash = new Hash();
            foreach (KeyValuePair<string, object> current in dictionary)
            {
                if (current.Value is Dictionary<string, object>)
                {
                    hash.Add(current.Key, Hash.FromDictionary((IDictionary<string, object>)current.Value));
                }
                else
                {
                    hash.Add(current);
                }
            }
            return hash;
        }
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

        private static IEnumerable<int> TestLoop(int token)
        {
            Console.WriteLine(token);
            while (true)
            {
                switch (token)
                {
                    case 5:
                        yield return 1;
                        token = 3;
                        break;
                    case 3:
                        token = 2;
                        continue;
                    case 2:
                        break;
                    default:
                        throw new InvalidDataException("");
                }
            }
        }

        public static async void WaitForSomething()
        {
            Console.WriteLine("before delay 1000");
            await Task.Delay(1000);
            Console.WriteLine("after delay 1000");
        }

        private static readonly Regex SelfBookmarkReferenceRegex = new Regex(@"^\#(?<bookmark>.+)", RegexOptions.Compiled);
        private static readonly Regex BookMarkReferenceRegex = new Regex(@"(?<fileReferencePath>.*)((?<extension>\.md|\.html)\#)(?<bookmark>.+)", RegexOptions.Compiled);
        private static readonly Regex QueryReferenceRegex = new Regex(@"(?<fileReferencePath>.*)((?<extension>\.md|\.html)\?)(?<query>.+)", RegexOptions.Compiled);
        private static readonly Regex NormalReferenceRegex = new Regex(@"(?<fileReferencePath>.*)(?<extension>\.md|\.html)$", RegexOptions.Compiled);

        private static string GetNormalizedFileReference(string reference, string replaceExtension)
        {
            var selfBookmarkMatch = SelfBookmarkReferenceRegex.Match(reference);
            if (selfBookmarkMatch.Success)
            {
                return selfBookmarkMatch.Value;
            }

            var bookmarkMatch = BookMarkReferenceRegex.Match(reference);
            if (bookmarkMatch.Success)
            {
                var originalExtension = bookmarkMatch.Groups["extension"].Value;
                return NormalizeBookmarkReference(bookmarkMatch, replaceExtension);
            }

            var queryMatch = QueryReferenceRegex.Match(reference);
            if (queryMatch.Success)
            {
                var originalExtension = queryMatch.Groups["extension"].Value;
                return NormalizeQueryReference(queryMatch, replaceExtension);
            }

            var normalMatch = NormalReferenceRegex.Match(reference);
            if (normalMatch.Success)
            {
                var originalExtension = normalMatch.Groups["extension"].Value;
                return NormalizeNormalReference(normalMatch, replaceExtension);
            }

            return reference;
        }

        private static string NormalizeBookmarkReference(Match match, string replaceExtension)
        {
            var referencePath = match.Groups["fileReferencePath"].Value;
            return string.Format("{0}{1}#{2}", referencePath.ToLower(), replaceExtension.ToLower(), match.Groups["bookmark"].Value);
        }

        private static string NormalizeQueryReference(Match match, string replaceExtension)
        {
            var referencePath = match.Groups["fileReferencePath"].Value;
            return string.Format("{0}{1}?{2}", referencePath.ToLower(), replaceExtension.ToLower(), match.Groups["query"].Value);
        }

        private static string NormalizeNormalReference(Match match, string replaceExtension)
        {
            var referencePath = match.Groups["fileReferencePath"].Value;
            return string.Format("{0}{1}", referencePath.ToLower(), replaceExtension.ToLower());
        }

        public enum PublishItemType
        {
            Unknown,
            XrefMap,
            FileMap
        }

        public class ItemToPublish
        {
            [JsonProperty("relative_path")]
            public string RelativePath { get; set; }

            [JsonProperty("version")]
            public string Version { get; set; }

            [JsonProperty("type")]
            public PublishItemType Type { get; set; }

            [JsonExtensionData]
            public Dictionary<string, object> Metadata { get; set; }
        }

        public class BuildManifest
        {
            [JsonProperty("items_to_publish")]
            public ItemToPublish[] ItemsToPublish { get; set; }

            public override string ToString()
            {
                return JsonUtility.ToJsonString(this);
            }
        }

        private static bool IsCurrentDepot(string currentDepotName, string depotName) => string.Equals(depotName, currentDepotName, StringComparison.OrdinalIgnoreCase);

        private static Task<KeyValuePair<string, string>> ret(string s)
        {
            return Task.Run(() => { return new KeyValuePair<string, string>(s, s); });
        }

        private static Task PrintAsync(int i)
        {
            return Task.Run(() => { Thread.Sleep(i * 10000); Console.WriteLine($"i is {i}"); });
        }

        private static string myret(string branch)
        {
            branch = "a";
            return null;
        }

        private static async Task<IReadOnlyList<string>> fff(List<string> ls)
        {
            return
   (from depotToFilePathPair in await ls.SelectInParallelAsync(depotName => ret(depotName))
    let filePath = depotToFilePathPair.Value
    where filePath != null
    let depotName = depotToFilePathPair.Key
    orderby IsCurrentDepot("5", depotName) descending, depotName
    select filePath)
    .ToList();
        }

        class Digit<T>
        {
            public Digit(T d) { val = d; }
            public T val;
            // ...other members

            // User-defined conversion from Digit to double
            public static implicit operator T(Digit<T> d)
            {
                return d.val;
            }
        }

        public static class BranchNames
        {
            /// <summary>
            /// Default Branch name
            /// </summary>
            public const string DefaultBranchName = "live";

            /// <summary>
            /// Default side by side branch name
            /// </summary>
            public const string DefaultSideBySideBranchName = "live-sxs";

            /// <summary>
            /// Master branch name
            /// </summary>
            public const string MasterBranch = "master";

            /// <summary>
            /// Branch created from <see cref="DefaultBranchName"/>
            /// </summary>
            public const string CheckoutFromDefaultBranchName = "master";

            /// <summary>
            /// Virtual Branch Name for Provision
            /// </summary>
            public const string ProvisionBranchName = "provision";

            /// <summary>
            /// Default branches
            /// </summary>
            public static string[] DefaultBranches = new string[] { DefaultBranchName, DefaultSideBySideBranchName };
        }

        public class VersionInfo
        {
            public VersionInfo(string versionFolder, string xrefMap)
            {
                VersionFolder = versionFolder;
                XRefMap = xrefMap;
            }

            [JsonProperty("version_folder")]
            public string VersionFolder { get; set; }

            [JsonProperty("xref_map")]
            public string XRefMap { get; set; }
        }

        private const string DefaultXrefMap = "xrefmap.yml";
        private const string XrefMapSuffix = ".xrefmap.yml";

        public static string MappingBranch(string branch)
        {
            Guard.ArgumentNotNullOrEmpty(branch, nameof(branch));

            return branch.EndsWith("-sxs") ? branch.Substring(0, branch.Length - "-sxs".Length) : branch;
        }

        [Serializable]
        public class ListWithStringFallback : List<string>
        {
            public ListWithStringFallback() : base()
            {
            }

            public ListWithStringFallback(IEnumerable<string> list) : base(list)
            {
            }
        }

        [Serializable]
        public class BuildJsonConfig
        {
            [JsonProperty("xrefService")]
            public ListWithStringFallback XRefServiceUrls { get; set; }
        }

        private sealed class BuildConfig
        {
            [JsonProperty("build")]
            public BuildJsonConfig Item { get; set; }
        }

        public static T GetConfig<T>(string configFile)
        {
            if (!File.Exists(configFile)) throw new FileNotFoundException($"Config file {configFile} does not exist!");

            return JsonUtility.Deserialize<T>(configFile);
        }

        public static void Filter(VersionInfo versionInfo)
        {
            versionInfo.VersionFolder = "new";
        }

        static void Main(string[] args)
        {
            var versionInf = new VersionInfo("old", "xref");
            Filter(versionInf);
            var content = GetConfig<BuildConfig>(@"C:\TestFiles\xrefmap\docfx.temp.json");
            var def = BranchNames.DefaultBranches;
            var branch1 = MappingBranch("live-sxs");
            var ps = Path.Combine("/", "/basepath", "ab");
            Uri uri;
            if (Uri.TryCreate("http://www.abc.com/base/a.html", UriKind.Relative, out uri))
            {
                Console.WriteLine("is relative");
            }
                var versionFolder = new VersionInfo("folder", null);
            File.WriteAllText(@"C:\TestFiles\xrefmap\filemap.now.json", JsonUtility.ToJsonString(versionFolder, true));
            var paths = new List<string> { "1", "2"};
            var joinedPath = string.Join(", ", paths);
            var dict = new Dictionary<int, int>();
            dict.Add(0, 0);
            dict.Add(1, 1);
            dict.Add(2, 2);
            dict.Remove(0);
            dict.Add(10, 10);

            foreach (var entry in dict)
            {
                Console.WriteLine(entry.Key);
            }

            Dictionary<string, VersionInfo> versionInfo = new Dictionary<string, VersionInfo>
            {
                {"testMoniker-1.0_justfortest", new VersionInfo("v2.0/", null) },
                {"testMoniker-1.1_justfortest", new VersionInfo("v2.1/", null) }
            };

            var xrefmaps = new List<string>
            {
                "xrefmap.yml",
                "testMoniker-1.0_justfortest.xrefmap.yml",
                "testMoniker-1.1_justfortest.xrefmap.yml"
            };

            var defaultXrefMap = xrefmaps.FirstOrDefault(x => string.Equals(x, DefaultXrefMap, StringComparison.OrdinalIgnoreCase));

            var defaultVersionInfo = !string.IsNullOrEmpty(defaultXrefMap) ? new VersionInfo(string.Empty, defaultXrefMap) : null;

            var xrefmapsWithVersion = xrefmaps.Where(x => !string.IsNullOrEmpty(x) && x.EndsWith(XrefMapSuffix));
            foreach (var xrefmapWithVersion in xrefmapsWithVersion)
            {
                var escapedVersion = xrefmapWithVersion.Substring(0, xrefmapWithVersion.Length - XrefMapSuffix.Length);
                if (!string.IsNullOrEmpty(escapedVersion))
                {
                    var unescapedversion = Uri.UnescapeDataString(escapedVersion);
                    VersionInfo versionInfoItem;
                    if (versionInfo.TryGetValue(unescapedversion, out versionInfoItem) && versionInfoItem != null)
                    {
                        versionInfoItem.XRefMap = xrefmapWithVersion;
                    }
                }
            }

            var branch = "live";
            myret(branch);
            Directory.CreateDirectory(@"c:\ab\c\text.txt");
            //webc.Get(new Uri("c:\\ab.txt"));
            var da = new string[] { "1" };
            var fir = da.First(x => x == "2");
            foreach (var fi in fir)
            {
                Console.WriteLine("");
            }
            Digit<string[]> dig = new Digit<string[]>(da);
            //This call invokes the implicit "double" operator
            string[] num = dig;

            //string snu = null;
            //var tsnu = (string)snu;
            //var t1 = PrintAsync(1);
            //var t2 = PrintAsync(2);
            //var t3 = PrintAsync(3);
            //var t4 = PrintAsync(4);
            //var t5 = PrintAsync(5);
            //var tasks = new List<Task> { t1, t2, t3 };
            //TaskHelper.WhenAll(tasks, 2).Wait();
            var depots = new List<string> { "1", "4", "2", "3" };
            var allDepots = depots.Concat(new List<string> { "6" });
            var dep = fff(depots).Result;

            var orderDepots = depots.OrderByDescending(depot => IsCurrentDepot("1", depot));
            var nr = FileUtility.NormalizeRelativePath("test.txt");
            var de = default(string);
            var manifestJson = File.ReadAllText(@"C:\Users\ychenu\Downloads\2929183a-17190aeb%5C201708210354455948-master\testMultipleVersion\test.json");
            var buildManifest = JsonUtility.FromJsonString<BuildManifest>(manifestJson);
            foreach (var item in buildManifest.ItemsToPublish)
            {
                if (item.Type == PublishItemType.XrefMap)
                {
                    Console.WriteLine($"{item.RelativePath} is xrefmap");
                }
            }
            IEnumerable<string> itemss = new List<string> {"1.json", "2.json", "3.json"};
            var itemsss = itemss.ToList();
            itemss.GenerateMtaJsonFilesAsync().Wait();
            var filename = Path.ChangeExtension("test\\test.md", ".mta.json");
            JsonUtility.ToJsonFile(filename, "test");
            var combined = Path.Combine("test\\index.md", ".mta.json");
            var loop = TestLoop(3).ToList();
            var version = "<abc>";
            var escapedata = Uri.EscapeDataString(version);
            var data = Uri.UnescapeDataString(escapedata);
            Dictionary<string, List<string>> repoByKey = new Dictionary<string, List<string>>();
            repoByKey["key1"] = new List<string> { "1" };
            var repos = repoByKey["key1"];
            repos.Add("2");
            File.WriteAllLines(@"D:\Data\DataFix\FixLocRepoConfig\test.txt", new List<string> {"1", "2"});
            File.AppendText(@"D:\Data\DataFix\FixLocRepoConfig\test.txt");
            var now = $"{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.log";
            var utcnow = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var pa = Path.Combine(@"c:\test\testfile", "RepositoryTableData.json");
            var dir = Path.GetDirectoryName(@"c:\test\testfile\abc.txt");
            Directory.CreateDirectory(dir);
            File.WriteAllText(@"c:\test\testfile\abc.txt", "test");
            var list = new List<int> { };
            var filter = list.Where(i => i == 3).ToList();
            var useAsync = ConfigurationManager.AppSettings["UseAsync"];
            var parallelism = -1;
            int.TryParse(ConfigurationManager.AppSettings["Parallelism"], out parallelism);
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);
                Console.WriteLine("sleep for 5s");
            }).Wait();
            var herf = "api/System.Web.UI.MobileControls.MobileListItem.OnBubbleEvent.html#System_Web_UI_MobileControls_MobileListItem_OnBubbleEvent_System_Object_System_EventArgs_";
            var rep = GetNormalizedFileReference(herf, string.Empty);
            var dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            dic.Add("a", 1);
            dic.Add("A", 40);
            dic.Add("context", new Dictionary<string, object> { { "1", 3 } });
            var hash = Hash.FromDictionary(dic);
            Console.WriteLine("before WaitForSomething");
            WaitForSomething();
            Console.WriteLine("after WaitForSomething");
            AsyncAwaitDemo.Get().Wait();
            var ie = new string[] { }.Where(i => i == "");
            var jjj = string.Join(",", null );
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
