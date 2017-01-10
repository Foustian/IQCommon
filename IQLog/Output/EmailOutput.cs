using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.IO;
using IQLog.Model;
using Newtonsoft.Json;

namespace IQLog.Output
{
    class EmailOutput : ILogOutput
    {
        public LogLevel MinLevel { get; set; }

        public string SmptServer { get; set; }
        public string From { get; set; }
        public List<string> To { get; set; }

        public string LogPath { get; set; }
        public int LogTailLength { get; set; }

        public EmailOutput()
        {
            LogTailLength = 2048;
        }

        public void WriteMessage(LogEvent evt)
        {
            if (evt.Level >= MinLevel)
            {
                // construct email body
                var builder = new StringBuilder();

                builder.Append("Time: ").Append(evt.Timestamp).AppendLine().AppendLine();

                builder.Append("Level: ").Append(evt.Level).AppendLine();
                if (evt.EventCode > 0)
                {
                    builder.Append("Code: ").Append(evt.EventCode).AppendLine();
                }
                builder.Append("Message: ").Append(evt.Message).AppendLine().AppendLine();

                builder.Append("Host IP: ").Append(IQLogger.Context.HostIP).AppendLine();
                builder.Append("Hostname: ").Append(IQLogger.Context.Hostname).AppendLine().AppendLine();

                var processTypeID = IQLogger.Context.ProcessTypeID;
                if (processTypeID.HasValue)
                {
                    builder.Append("Process Type ID: ").Append(processTypeID).AppendLine();
                }
                var processType = IQLogger.Context.ProcessType;
                if (!string.IsNullOrWhiteSpace(processType))
                {
                    builder.Append("Process Name: ").Append(processType).AppendLine().AppendLine();
                }

                builder.AppendLine("Additional Data:");
                builder.AppendLine(JsonConvert.SerializeObject(evt.Values, Formatting.Indented));

                if (LogPath != null && LogPath.Length > 0 && File.Exists(LogPath))
                {
                    builder.AppendLine().AppendLine("Log Tail:").AppendLine();
                    copyLogTail(LogPath, builder);
                }

                // send email
                SmtpClient client = new SmtpClient(SmptServer);
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(From);
                foreach (string address in To)
                {
                    mail.Bcc.Add(address);
                }
                mail.Subject = String.Format("{0}: {1} - {2} - {3}", evt.Level, IQLogger.Context.Hostname, IQLogger.Context.LogName, evt.Message);
                mail.Body = builder.ToString();

                client.Send(mail);
            }
        }

        private void copyLogTail(string logPath, StringBuilder output)
        {
            var stream = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var reader = new StreamReader(stream))
            {
                if (reader.BaseStream.Length > LogTailLength)
                {
                    reader.BaseStream.Seek(-LogTailLength, SeekOrigin.End);
                }
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    output.AppendLine(line);
                }
            }
        }
    }
}
