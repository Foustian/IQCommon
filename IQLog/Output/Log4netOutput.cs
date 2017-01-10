using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using IQLog.Model;

namespace IQLog.Output
{
    class Log4netOutput : LineOutput
    {
        protected override void WriteLine(LogLevel level, string line)
        {
            ILog logger = LogManager.GetLogger(IQLogger.Context.LogName, "IQLogger");
            if (logger != null)
            {
                switch (level)
                {
                    case LogLevel.DEBUG:
                        logger.Debug(line);
                        break;
                    case LogLevel.INFO:
                        logger.Info(line);
                        break;
                    case LogLevel.WARNING:
                        logger.Warn(line);
                        break;
                    case LogLevel.ERROR:
                        logger.Error(line);
                        break;
                    case LogLevel.FATAL:
                        logger.Fatal(line);
                        break;
                    default:
                        logger.Debug(line);
                        break;
                }
            }
        }
    }
}
