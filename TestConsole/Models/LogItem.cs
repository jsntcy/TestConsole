namespace TestConsole.Models
{
    using System;
    using System.ComponentModel;

    using Newtonsoft.Json;

    using TestConsole.Utilities;

    public enum MessageSeverity
    {
        Error,
        Warning,
        Info,
        Verbose,
        Diagnostic
    }

    public enum LogItemType
    {
        Unspecified,
        System,
        User,
    }

    public class LogItem
    {
        /// <summary>
        /// Message to log
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Project class that throw the exception
        /// </summary>
        [JsonProperty("source")]
        public string Source { get; set; }

        /// <summary>
        /// Processing file
        /// </summary>
        [JsonProperty("file")]
        public string File { get; set; }

        /// <summary>
        /// Line number in file
        /// </summary>
        [JsonProperty("line")]
        public int? Line { get; set; }

        /// <summary>
        /// Message Severity
        /// </summary>
        [JsonProperty("message_severity")]
        public MessageSeverity MessageSeverity { get; set; }

        /// <summary>
        /// Log code of the item
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Log time
        /// </summary>
        [JsonProperty("date_time")]
        public DateTime DateTime { get; set; }

        /// <summary>
        /// The type of the log item, could be user or system
        /// </summary>
        [JsonProperty("log_item_type")]
        public LogItemType LogItemType { get; set; }

        public LogItem()
        {
        }

        public LogItem(
            string message,
            string source,
            string file,
            MessageSeverity messageSeverity,
            LogItemType logItemType)
            : this(message, source, file, null, DateTime.UtcNow, messageSeverity, null, logItemType)
        {
        }

        public LogItem(
            string message,
            string source,
            string file,
            int line,
            MessageSeverity messageSeverity,
            LogItemType logItemType)
            : this(message, source, file, line, DateTime.UtcNow, messageSeverity, null, logItemType)
        {
        }

        public LogItem(
            string message,
            string source,
            string file,
            MessageSeverity messageSeverity,
            string code,
            LogItemType logItemType)
            : this(message, source, file, null, DateTime.UtcNow, messageSeverity, code, logItemType)
        {
        }

        public LogItem(
            string message,
            string source,
            string file,
            int line,
            MessageSeverity messageSeverity,
            string errorCode,
            LogItemType logItemType)
            : this(message, source, file, line, DateTime.UtcNow, messageSeverity, errorCode, logItemType)
        {
        }

        [JsonConstructor]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public LogItem(
            string message,
            string source,
            string file,
            int? line,
            DateTime dateTime,
            MessageSeverity messageSeverity,
            string code,
            LogItemType logItemType)
        {
            Guard.ArgumentNotNull(message, nameof(message));

            Message = message;
            Source = source;
            File = file;
            Line = line;
            DateTime = dateTime;
            MessageSeverity = messageSeverity;
            Code = code;
            LogItemType = logItemType;
        }
    }
}
