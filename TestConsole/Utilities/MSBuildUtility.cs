namespace TestConsole.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;

    using Microsoft.Build.Framework;

    public static class MSBuildUtility
    {
        public static T FromTaskItem<T>(ITaskItem taskItem, string metadataName)
        {
            var objString = taskItem.GetMetadata(metadataName);
            var obj = FromDataContractStringStrict<T>(objString);
            return obj;
        }

        private static T FromDataContractStringStrict<T>(string xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException(nameof(xml));
            }

            DataContractSerializer dcs = new DataContractSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(ms))
            {
                sw.Write(xml);
                sw.Flush();
                ms.Position = 0;
                return (T)dcs.ReadObject(ms);
            }
        }
        public static void TestTaskItem()
        {
            var publishAdditionalInformation = new Dictionary<string, string>();
            publishAdditionalInformation.Add("BranchName", "master");
            var publishAdditionalInformationTaskItem = new Microsoft.Build.Utilities.TaskItem("PublishAdditionalInformation", publishAdditionalInformation);
            publishAdditionalInformationTaskItem.SetMetadata("ispackageupdated", "true");
            var metadata = new Dictionary<string, string>();
            foreach (DictionaryEntry m in publishAdditionalInformationTaskItem.CloneCustomMetadata())
            {
                metadata.Add(m.Key.ToString(), m.Value?.ToString());
            }
            var bn = FromTaskItem<string>(publishAdditionalInformationTaskItem, "BranchName");
            var dt = publishAdditionalInformationTaskItem.CloneCustomMetadata();
            foreach (DictionaryEntry entry in dt)
            {
                Console.WriteLine(entry.Key);
            }
        }
    }
}
