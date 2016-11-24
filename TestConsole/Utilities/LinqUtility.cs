namespace TestConsole.Utilities
{
    using System.Linq;

    public enum ManifestItemType
    {
        Content,
        Resource,
        Toc
    }

    public class ManifestItem
    {
        public ManifestItemType Type { get; set; }

        public string Name { get; set; }
    }

    class LinqUtility
    {
        public static void SortByBoolean()
        {
            var manifestItems = new ManifestItem[]
            {
                new ManifestItem
                {
                    Type = ManifestItemType.Content,
                    Name = "item1"
                },
                new ManifestItem
                {
                    Type = ManifestItemType.Toc,
                    Name = "item2"
                },
                new ManifestItem
                {
                    Type = ManifestItemType.Resource,
                    Name = "item3"
                },
                new ManifestItem
                {
                    Type = ManifestItemType.Content,
                    Name = "item4"
                },
                new ManifestItem
                {
                    Type = ManifestItemType.Toc,
                    Name = "item5"
                },
                new ManifestItem
                {
                    Type = ManifestItemType.Resource,
                    Name = "item6"
                },
            };

            //var orderedItems1 = (from manifestItem in manifestItems
            //          orderby manifestItem.Type == ManifestItemType.Resource descending
            //          select manifestItem).ToArray();

            var orderedItems = manifestItems.OrderByDescending(item => item.Type == ManifestItemType.Resource).ToArray();
        }
    }
}
