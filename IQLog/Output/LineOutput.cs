using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using IQLog.Model;

namespace IQLog.Output
{
    abstract class LineOutput : ILogOutput
    {
        private const string SEPERATOR = " ";
        private const string PLACEHOLDER = "-";

        public void WriteMessage(LogEvent evt)
        {
            var builder = new StringBuilder();
            var processTypeID = IQLogger.Context.ProcessTypeID;

            // Standard fields are seperated by spaces
            builder.Append(evt.Timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
                   .Append(SEPERATOR)
                   .Append(placeholderString(IQLogger.Context.HostIP))
                   .Append(SEPERATOR)
                   .Append(placeholderString(IQLogger.Context.Hostname))
                   .Append(SEPERATOR)
                   .Append(IQLogger.Context.ProcessType ?? (processTypeID.HasValue ? processTypeID.Value.ToString() : "-"))
                   .Append(SEPERATOR)
                   .Append(placeholderString(IQLogger.Context.LogName))
                   .Append(SEPERATOR)
                   .Append(placeholderString(evt.Level.ToString()))
                   .Append(SEPERATOR);

            if (evt.EventCode > 0)
            {
                builder.Append(evt.EventCode).Append(SEPERATOR);
            }
            else
            {
                builder.Append('-').Append(SEPERATOR);
            }

            // Append message in quotes to simplify pattern matching
            if (String.IsNullOrEmpty(evt.Message))
            {
                builder.Append("\"\"");
            }
            else
            {
                // Removing double quotes from the message content allows for much simpler filter patterns in Logstash
                builder.Append('"').Append(evt.Message.Replace('"', '\'')).Append('"');
            }

            if (evt.Values != null && evt.Values.Count > 0)
            {
                builder.Append(SEPERATOR).Append(JsonConvert.SerializeObject(evt.Values, Formatting.None));
            }

            WriteLine(evt.Level, builder.ToString());
        }

        protected abstract void WriteLine(LogLevel level, string line);

        private string placeholderString(string item)
        {
            return String.IsNullOrWhiteSpace(item) ? PLACEHOLDER : item;
        }
    }
}
