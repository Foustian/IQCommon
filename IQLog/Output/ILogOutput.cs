using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQLog.Model;

namespace IQLog.Output
{
    interface ILogOutput
    {
        void WriteMessage(LogEvent evt);
    }
}
