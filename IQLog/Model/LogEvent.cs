using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace IQLog.Model
{
    class LogEvent
    {
        public DateTime Timestamp { get; set; }
        public short? EventCode { get; set; }
        public string Message { get; set; }
        public LogLevel Level { get; set; }

        public Dictionary<string, object> Values { get; private set; }

        public LogEvent()
        {
            Timestamp = DateTime.UtcNow;
            Values = new Dictionary<string, object>();
        }
    }
}
