using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Configuration;
using IQLog.Configuration;
using IQLog.Output;
using IQLog.Model;
using log4net;
using log4net.Repository;
using System.IO;

namespace IQLog
{
    /// <summary>
    /// Primary class for writing logs
    /// </summary>
    public static class IQLogger
    {
        private static List<ILogOutput> outputs;
        private static Dictionary<short, EventType> EventTypes;

        static IQLogger()
        {
            outputs = new List<ILogOutput>() { new ConsoleOutput() };
        }

        public static EventBuilder BuildDebug(string message, params object[] messageParams)
        {
            return new EventBuilder(LogLevel.DEBUG, null, message, messageParams);
        }

        public static void Debug(string message, params object[] messageParams)
        {
            BuildDebug(message, messageParams).Submit();
        }

        public static void Debug(string message, Exception ex, params object[] messageParams)
        {
            BuildDebug(message, messageParams).SetException(ex).Submit();
        }

        public static EventBuilder BuildEvent(short eventCode, params object[] messageParams)
        {
            EventType type = GetEventData(eventCode);
            if (type != null)
            {
                return new EventBuilder((LogLevel)type.LogLevel, eventCode, type.Message, messageParams);
            }
            var builder = new EventBuilder(LogLevel.ERROR, eventCode, "Unrecognized log event code " + eventCode);
            if (messageParams != null && messageParams.Length > 0)
            {
                builder.SetValue("params", messageParams);
            }
            return builder;
        }

        public static void Event(short eventCode, params object[] messageParams)
        {
            BuildEvent(eventCode, messageParams).Submit();
        }

        public static void Event(short eventCode, Exception ex, params object[] messageParams)
        {
            BuildEvent(eventCode, messageParams).SetException(ex).Submit();
        }

        internal static void Submit(LogEvent evt)
        {
            if (outputs != null && outputs.Count > 0)
            {
                foreach (ILogOutput output in outputs)
                {
                    try
                    {
                        output.WriteMessage(evt);
                    }
                    catch (Exception ex)
                    {
                        string message = "Output failed to write log message:  [" + ex.GetType().FullName + "] " + ex.Message + " - " + ex.StackTrace;
                        Console.Error.WriteLine(message);
                        LogManager.GetLogger("IQLogger").Error(message);
                    }
                }
            }
            else
            {
                string message = "No log outputs configured.";
                Console.Error.WriteLine(message);
                LogManager.GetLogger("IQLogger").Error(message);
            }
        }

        public static void Configure(string logName)
        {
            Context.ProcessName = logName;
            log4net.Config.XmlConfigurator.Configure(LogManager.CreateRepository(logName));
            LoadConfig();
            LoadData();
        }

        public static void ConfigureThread(string logName)
        {
            Context.ThreadName = logName;
            log4net.Config.XmlConfigurator.Configure(LogManager.CreateRepository(logName));
        }

        public static void ShutdownThread()
        {
            var logName = Context.LogName;
            if (!String.IsNullOrWhiteSpace(logName))
            {
                LogManager.ShutdownRepository(logName);
            }
        }
        
        public static void LoadData()
        {
            try
            {
                var context = new IQLogDataContext();

                var eventEnumerator = context.EventTypeSelectAll().GetEnumerator();
                var eventDict = new Dictionary<short, EventType>();
                while (eventEnumerator.MoveNext())
                {
                    var evt = eventEnumerator.Current;
                    eventDict.Add(evt.EventCode, evt);
                }
                EventTypes = eventDict;

                var processTypeID = Context.ProcessTypeID;
                if (processTypeID.HasValue && processTypeID.Value > 0)
                {
                    var nameOutput = new System.Data.Objects.ObjectParameter("Name", typeof(string));
                    context.ProcessTypeSelectNameById(processTypeID.Value, nameOutput);
                    if (nameOutput.Value != null)
                    {
                        Context.ProcessType = (string)nameOutput.Value;
                    }
                    else
                    {
                        new EventBuilder(LogLevel.WARNING, null, "Process type " + processTypeID + " not found").Submit();
                    }
                }
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                new EventBuilder(LogLevel.ERROR, null, "Failed to load data for IQLogger.").SetException(ex).Submit();
            }
        }

        private static void LoadConfig()
        {
            Console.Out.WriteLine("LoadConfig()");
            try
            {
                // Process information
                var processInfoSection = (ProcessInfoSection)ConfigurationManager.GetSection("IQLog/ProcessInfo");
                if (processInfoSection != null)
                {
                    Console.Out.WriteLine("processInfoSection");
                    Context.ProcessTypeID = processInfoSection.ProcessType;
                }

                // Init outputs with local file output
                Console.Out.WriteLine("Init outputs");
                outputs = new List<ILogOutput>();

                // Email output configuration
                var emailSection = (EmailOutputSection)ConfigurationManager.GetSection("IQLog/EmailOutput");
                if (emailSection != null)
                {
                    Console.Out.WriteLine("emailSection");
                    EmailOutput emailOutput = new EmailOutput();
                    emailOutput.SmptServer = emailSection.SMTPServer;
                    emailOutput.From = emailSection.FromEmail;
                    emailOutput.To = new List<string>() { emailSection.ToAddress };
                    var levelStr = emailSection.MinLevel;
                    LogLevel minLevel;
                    if (Enum.TryParse<LogLevel>(emailSection.MinLevel, true, out minLevel))
                    {
                        emailOutput.MinLevel = minLevel;
                    }
                    emailOutput.LogPath = emailSection.LogPath;
                    emailOutput.LogTailLength = emailSection.LogTailLength;
                    outputs.Add(emailOutput);
                }
                outputs.Add(new Log4netOutput());
            }
            catch (Exception ex)
            {
                string message = "Failed to initialize IQLogger: [" + ex.GetType().FullName + "] " + ex.Message + " - " + ex.StackTrace;
                Console.Error.WriteLine(message);
                LogManager.GetLogger("IQLogger").Error(message);
            }
        }

        private static EventType GetEventData(short eventCode)
        {
            try
            {
                return EventTypes[eventCode];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static class Context
        {
            static Context()
            {
                HostIP = getLocalIP();
                Hostname = Environment.MachineName;
            }

            public static string HostIP
            {
                get
                {
                    return (string)GlobalContext.Properties["IP"];
                }
                set
                {
                    GlobalContext.Properties["IP"] = value;
                }
            }

            public static string Hostname
            {
                get
                {
                    return (string)GlobalContext.Properties["Hostname"];
                }
                set
                {
                    GlobalContext.Properties["Hostname"] = value;
                }
            }

            public static short? ProcessTypeID
            {
                get
                {
                    return (short?)GlobalContext.Properties["ProcessTypeID"];
                }
                set
                {
                    GlobalContext.Properties["ProcessTypeID"] = value;
                }
            }

            public static string ProcessType
            {
                get
                {
                    return (string)GlobalContext.Properties["ProcessType"];
                }
                internal set
                {
                    GlobalContext.Properties["ProcessType"] = value;
                }
            }

            public static string LogName
            {
                get
                {
                    return ThreadName ?? ProcessName;
                }
            }

            internal static string ProcessName
            {
                get
                {
                    return (string)GlobalContext.Properties["LogName"];
                }
                set
                {
                    GlobalContext.Properties["LogName"] = value;
                }
            }

            internal static string ThreadName
            {
                get
                {
                    return (string)ThreadContext.Properties["LogName"];
                }
                set
                {
                    ThreadContext.Properties["LogName"] = value;
                }
            }

            private static string getLocalIP()
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return null;
            }
        }
    }
}
