namespace TestConsole
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class WorkflowRequest
    {
        public string BaseString { get; set; }

        [JsonExtensionData]
        public JObject RequestPayload { get; set; }
    }

    public class PushWorkflowRequest : WorkflowRequest
    {
        public string PushString { get; set; }
    }

    public class DependencyPushWorkflowRequest : PushWorkflowRequest
    {
        public bool ForcePublish { get; set; }

        public List<string> BranchesToFilter { get; set; }
    }

    public static class MessageGeneratorHelper
    {
        public static T ToSpecificWorkflowRequest<T>(this WorkflowRequest request)
            where T : WorkflowRequest
            => JObject.FromObject(request).ToObject<T>();
    }

    public class ObjectConverter
    {
        private static void TestConvert(WorkflowRequest request)
        {
            TestConvert1(request.ToSpecificWorkflowRequest<PushWorkflowRequest>());
        }

        private static void TestConvert1(PushWorkflowRequest request)
        {
            var dpwfr = request.ToSpecificWorkflowRequest<DependencyPushWorkflowRequest>();

            List<string> de = new List<string> { "live", "123" };
        }
    }

    internal class BuildIdJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BuildId);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                return null;
            }

            return new BuildId((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((BuildId)value).ToString());
        }

        public override bool CanWrite => true;
    }

    [JsonConverter(typeof(BuildIdJsonConverter))]
    public class BuildId : IComparable<BuildId>
    {
        private readonly string _id;

        #region Constructors

        public BuildId(string id)
        {
            ValidateId(id);
            _id = id;
        }

        internal BuildId(DateTime dateTime, string branchName)
        {
            _id = string.Format("{0}-{1}", dateTime.ToString("yyyyMMddHHmmssffff"), branchName);
        }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var target = obj as BuildId;
            if (target == null)
            {
                return false;
            }

            return string.Equals(_id, target._id);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public int CompareTo(BuildId other)
        {
            if (other == null)
            {
                return 1;
            }

            return _id.CompareTo(other._id);
        }

        public override string ToString()
        {
            return _id;
        }

        public static explicit operator string(BuildId id)
        {
            return id?.ToString();
        }

        public static explicit operator BuildId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return new BuildId(id);
        }

        #endregion

        #region Private Methods

        private static void ValidateId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id), "id is null or empty");
            }

            int index = id.IndexOf('-');
            if (index > -1)
            {
                string dateTimePart = id.Substring(0, index);
                string branchPart = id.Substring(index + 1);

                DateTime dt;
                if (DateTime.TryParseExact(dateTimePart, "yyyyMMddHHmmssffff", new CultureInfo("en-US"), DateTimeStyles.None, out dt)
                    && branchPart.Length > 0)
                {
                    return;
                }
            }

            throw new ArgumentException($"Id {id} doesn't fit the right format 'timestamp-branch', the format of timestamp is 'yyyMMddHHmmssffff'.");
        }

        #endregion
    }

    public abstract class Id : IComparable<Id>
    {
        private readonly Guid _guid;

        #region Constructors

        protected Id(string id)
        {
            this._guid = new Guid(id);
        }

        protected Id(Guid guid)
        {
            this._guid = guid;
        }

        #endregion  // Constructors

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var target = obj as Id;
            if (target == null)
            {
                return false;
            }

            return this._guid == target._guid;
        }

        public int CompareTo(Id other)
        {
            if (other == null)
            {
                return 1;
            }

            return this._guid.CompareTo(other._guid);
        }

        public override string ToString()
        {
            return this._guid.ToString("D");
        }

        public override int GetHashCode()
        {
            return this._guid.GetHashCode();
        }

        public Guid ToGuid()
        {
            return this._guid;
        }

        #region Utility Methods

        public static bool operator ==(Id a, Id b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Id a, Id b)
        {
            return !(a == b);
        }

        #endregion // Utility Methods

        #endregion // Public Methods
    }

    [JsonConverter(typeof(RepoIdJsonConverter))]
    public class RepoId : Id, IComparable<RepoId>
    {
        #region Constructors

        public RepoId(string id) : base(id)
        {
        }

        public RepoId(Guid guid) : base(guid)
        {
        }

        #endregion  // Constructors

        #region Public Methods

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int CompareTo(RepoId other)
        {
            return base.CompareTo(other);
        }

        #region Utility Methods

        public static bool TryParse(string id, out RepoId repositoryId)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(id, nameof(id));
            }

            repositoryId = null;
            try
            {
                repositoryId = new RepoId(id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool operator ==(RepoId a, RepoId b)
        {
            return (Id)a == (Id)b;
        }

        public static bool operator !=(RepoId a, RepoId b)
        {
            return !(a == b);
        }

        public static explicit operator string(RepoId id)
        {
            return id == null ? null : id.ToString();
        }

        public static explicit operator RepoId(string id)
        {
            return string.IsNullOrEmpty(id) ? null : new RepoId(id);
        }

        #endregion // Utility Methods

        #endregion // Public Methods
    }

    internal class RepoIdJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(RepoId);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                return null;
            }

            return new RepoId((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((RepoId)value).ToString());
        }

        public override bool CanWrite => true;
    }
}
