namespace TestConsole.Utilities
{
    using System;
    using System.IO;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Collections.Generic;

    public class MasterRedirection
    {
        [JsonProperty("redirections")]
        public List<RedirectionEntry> Entries { get; set; }
    }

    public class RedirectionEntry
    {
        [JsonProperty("source_path")]
        public string SourcePath { get; set; }

        [JsonProperty("redirect_url")]
        public string RedirectUrl { get; set; }

        [JsonProperty("redirect_document_id")]
        public bool? RedirectDocumentId { get; set; }
    }

    public static class JsonUtility
    {
        public static T ReadFromJsonFile<T>(string filePath)
        {
            Guard.ArgumentNotNull(filePath, nameof(filePath));

            if (!File.Exists(filePath))
            {
                return default(T);
            }

            using (var stream = File.OpenRead(filePath))
            {
                try
                {
                    return JsonUtility.FromJsonStream<T>(stream);
                }
                catch (ArgumentException)
                {
                    return default(T);
                }
            }
        }

        public static T FromJsonString<T>(string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (JsonReaderException jre)
            {
                throw new ArgumentException("Invalid json string", jre);
            }
            catch (JsonSerializationException jse)
            {
                throw new ArgumentException($"Invalid json string for type {typeof(T)}.", jse);
            }
        }

        public static T FromJsonStream<T>(Stream stream)
        {
            try
            {
                using (var sr = new StreamReader(stream))
                using (var reader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize<T>(reader);
                }
            }
            catch (JsonReaderException jre)
            {
                throw new ArgumentException("Invalid json string", jre);
            }
            catch (JsonSerializationException jse)
            {
                throw new ArgumentException($"Invalid json string for type {typeof(T)}.", jse);
            }
        }

        public static Stream ToJsonStream(object value)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            var jsonSerializer = new JsonSerializer();

            jsonSerializer.Serialize(streamWriter, value);

            streamWriter.Flush();
            memoryStream.Position = 0;

            return memoryStream;
        }

        public static string ToIndentedJsonString(object value, bool ignoreNullValue = true)
        {
            var setting = new JsonSerializerSettings
            {
                Converters = new JsonConverter[] { new StringEnumConverter() },
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = ignoreNullValue ? NullValueHandling.Ignore : NullValueHandling.Include
            };

            // Note that when JsonConvert simple types, quotes will be added to the value
            return JsonConvert.SerializeObject(value, setting);
        }
    }
}
