using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQLog.Model;

namespace IQLog.Output
{
    class ConsoleOutput : LineOutput
    {
        protected override void WriteLine(LogLevel level, string line)
        {
            if (level == LogLevel.ERROR || level == LogLevel.FATAL)
            {
                Console.Error.WriteLine(line);
            }
            else
            {
                Console.Out.WriteLine(line);
            }
        }
    }
}
