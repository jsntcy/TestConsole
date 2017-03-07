namespace TestConsole.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    class DictionaryUtility
    {
        public static void TestOrder()
        {
            var dict = new Dictionary<Guid, Guid>();
            Guid a = Guid.Empty, b = a, c = a, d = a;

            for (int i = 0; i < 1000; ++i)
            {
                var guid = Guid.NewGuid();
                dict.Add(guid, guid);

                if (dict.Last().Key != guid)
                    Console.WriteLine("Failed at iteration " + i);

                if (d != Guid.Empty)
                    dict.Remove(d);

                d = c;
                c = b;
                b = a;
                a = guid;
            }
        }

        public static void TestTryGetValue()
        {
            var dicmy = new Dictionary<string, string>
            {
            };

            string isPackageUploadedString;
            if (dicmy.TryGetValue("isUploaded", out isPackageUploadedString) && !string.IsNullOrEmpty(isPackageUploadedString))
            {
                bool isPackageUploaded;
                if (bool.TryParse(isPackageUploadedString, out isPackageUploaded))
                {
                    var isUploaded = isPackageUploaded;
                }
            }
        }
    }
}
