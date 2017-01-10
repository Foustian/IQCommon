using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQLog.Model;

namespace IQLog
{
    public class EventBuilder
    {
        private LogEvent evt;

        internal EventBuilder(LogLevel level, short? code, string message, params object[] messageParams)
        {
            evt = new LogEvent();
            evt.Level = level;
            evt.EventCode = code;
            evt.Message = message;
            formatMessage(message, messageParams);
        }

        public EventBuilder SetTimestamp(DateTime timestamp)
        {
            evt.Timestamp = timestamp;
            return this;
        }

        public EventBuilder SetException(string key, Exception ex)
        {
            evt.Values.Add(key, mapException(ex));
            return this;
        }

        public EventBuilder SetException(Exception ex)
        {
            return SetException("exception", ex);
        }

        public EventBuilder SetValue(string key, object value)
        {
            evt.Values.Add(key, value);
            return this;
        }

        public EventBuilder SetActionNeeded(string action)
        {
            return SetValue("actionNeeded", action);
        }

        public void Submit()
        {
            IQLogger.Submit(evt);
        }

        private void formatMessage(string format, params object[] args)
        {
            try
            {
                evt.Message = String.Format(format, args);
            }
            catch (FormatException)
            {
                SetValue("messageFormat", format);
                SetValue("originalLevel", evt.Level);

                evt.Message = "Log message format error";
                evt.Level = LogLevel.ERROR;
            }
            catch (ArgumentNullException)
            {
                evt.Message = "Null format ";
                evt.Level = LogLevel.ERROR;
            }
        }

        private Dictionary<string, object> mapException(Exception ex)
        {
            var map = new Dictionary<string, object>(ex.InnerException != null ? 5 : 4);
            map.Add("type", ex.GetType().FullName);
            map.Add("message", ex.Message);
            map.Add("source", ex.Source);
            map.Add("stackTrace", ex.StackTrace);
            if (ex.InnerException != null)
            {
                map.Add("innerException", mapException(ex.InnerException));
            }
            return map;
        }
    }
}
