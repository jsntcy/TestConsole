namespace TestConsole.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Item
    {
        public int? Number { get; set; }
        public string File { get; set; }
        public GitChangeType Type { get; set; }
    }

    public enum GitChangeType
    {
        Created,
        Updated,
        Deleted
    }

    public enum ManifestItemType
    {
        Content,
        Resource,
        Toc
    }

    public enum PublishStateType
    {
        // the document publish status is unknown. Correspond to the unknown status of cache.
        Unknown = 0x0,

        // the document has no change and not published
        Unpublished,

        // the document has change and published
        Published,

        // the document is deleted
        Deleted,
    }

    public class ManifestItem
    {
        public ManifestItemType Type { get; set; }

        public string Name { get; set; }
    }

    public class PublishManifestItem
    {
        public string Original { get; set; }

        public PublishStateType PublishState { get; set; }
    }

    public class Student
    {
        public string First { get; set; }
        public string Last { get; set; }
        public int ID { get; set; }
    }

    public static class LinqUtility
    {
        public static List<Student> GetStudents()
        {
            // Use a collection initializer to create the data source. Note that each element
            //  in the list contains an inner sequence of scores.
            List<Student> students = new List<Student>
            {
               new Student {First="Svetlana", Last="Omelchenko", ID=111},
               new Student {First="Claire", Last="O'Donnell", ID=112},
               new Student {First="Sven", Last="Mortensen", ID=113},
               new Student {First="Cesar", Last="Garcia", ID=114},
               new Student {First="Debra", Last="Garcia", ID=115}
            };

            return students;
        }

        public static void SortByMultipleKeys()
        {
            // Create the data source.
            List<Student> students = GetStudents();

            // Create the query.
            IEnumerable<Student> sortedStudents =
                from student in students
                orderby student.Last ascending, student.First ascending
                select student;

            // Execute the query.
            Console.WriteLine("sortedStudents:");
            foreach (Student student in sortedStudents)
                Console.WriteLine(student.Last + " " + student.First);

            Console.WriteLine("\r\n");

            // Create the query.
            IEnumerable<Student> sortedStudents1 =
                from student in students
                orderby student.Last, student.First
                select student;

            // Execute the query.
            Console.WriteLine("sortedStudents1:");
            foreach (Student student in sortedStudents1)
                Console.WriteLine(student.Last + " " + student.First);
        }

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

            var orderedItems1 = (from manifestItem in manifestItems
                                 orderby manifestItem.Type == ManifestItemType.Resource descending
                                 select manifestItem).ToArray();

            //var orderedItems = manifestItems.OrderByDescending(item => item.Type == ManifestItemType.Resource).ToArray();
        }

        public static void SortByValue()
        {
            Dictionary<string, GitChangeType> changes = new Dictionary<string, GitChangeType>
            {
                {"3", GitChangeType.Created },
                {"1", GitChangeType.Deleted },
                {"4", GitChangeType.Updated },
                {"5", GitChangeType.Deleted }
            };
            if (changes.ContainsKey("7"))
            {
                var changeType = changes["7"];
            }
            GitChangeType type1;
            if (changes.TryGetValue("", out type1))
            {
                Console.WriteLine("");
            }
            var wh = new List<string> { }.FirstOrDefault(change => change == "sd");
            var newChanges = changes
                .OrderByDescending(change => change.Value);
            foreach (var newChange in newChanges)
            {
                Console.WriteLine(newChange.Key + ":" + newChange.Value);
            }


            var items = new List<Item>
            {
                new Item
                {
                    File ="a.txt",
                    Number = 2
                },
                new Item
                {
                    Number = 2,
                    Type = GitChangeType.Updated
                },
                new Item
                {
                    File = "a.txt",
                    Number = 1
                },
                new Item
                {
                    File = "b.txt",
                    Number = 1
                },
                new Item
                {
                    Type = GitChangeType.Updated
                },
                new Item
                {
                    Number = 1,
                    Type = GitChangeType.Created
                },
                new Item
                {
                    File = "a.txt",
                    Type = GitChangeType.Created
                }
            };

            var sl = new List<string>();
            var filter = sl.Where(str => str.StartsWith("a") && str[2] == 'r');

            var displayItems = items
                    .GroupBy(item => item.File)
                    .OrderBy(group => group.Key)
                    .SelectMany(group => group.OrderBy(item => item.Number))
                    .ToList();
            Func<Item, int?> func = item11 => 2;
            var newItems = items.OrderBy(item => item.Number).ToArray();
            Action<int> action = Console.WriteLine;

            Open(5, action);
        }

        public static void Open(int count, Action<int> func)
        {
            count++;
            func(count);
            count--;
        }

        public static void SortPublishFiles()
        {
            PublishManifestItem[] publishManifestItems = new PublishManifestItem[]
            {
                new PublishManifestItem
                {
                    Original = "A",
                    PublishState = PublishStateType.Published
                },
                new PublishManifestItem
                {
                    Original = "B",
                    PublishState = PublishStateType.Unpublished
                },
                new PublishManifestItem
                {
                    Original = "C",
                    PublishState = PublishStateType.Published
                },
                new PublishManifestItem
                {
                    Original = "D",
                    PublishState = PublishStateType.Unknown
                },
            };

            string[] publishedFiles = (from publishManifestItem in publishManifestItems
                                       let file = publishManifestItem.Original
                                       let errorCount = GetErrorCount(file)
                                       orderby errorCount descending, publishManifestItem.PublishState == PublishStateType.Published descending
                                       select file
                           )
                           .ToArray();

            foreach (var file in publishedFiles)
            {
                Console.WriteLine(file);
            }
        }

        public static int GetErrorCount(string file)
        {
            switch (file)
            {
                case "A":
                    return 2;
                case "B":
                    return 0;
                case "C":
                    return 0;
                case "D":
                    return 2;
                default:
                    return 0;
            }
        }
    }
}
