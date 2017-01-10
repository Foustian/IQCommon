using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace IQLog.Configuration
{
    public class EmailOutputSection : ConfigurationSection
    {
        [ConfigurationProperty("SMTPServer", IsRequired = true)]
        public string SMTPServer
        {
            get
            {
                return (string)this["SMTPServer"];
            }
            set
            {
                this["SMTPServer"] = value;
            }
        }

        [ConfigurationProperty("FromAddress", IsRequired = true)]
        public string FromEmail
        {
            get
            {
                return (string)this["FromAddress"];
            }
            set
            {
                this["FromAddress"] = value;
            }
        }

        [ConfigurationProperty("ToAddress", IsRequired = true)]
        public string ToAddress
        {
            get
            {
                return (string)this["ToAddress"];
            }
            set
            {
                this["ToAddress"] = value;
            }
        }

        [ConfigurationProperty("MinLevel", IsRequired = true)]
        public string MinLevel
        {
            get
            {
                return (string)this["MinLevel"];
            }
            set
            {
                this["MinLevel"] = value;
            }
        }

        [ConfigurationProperty("LogPath", IsRequired = true)]
        public string LogPath
        {
            get
            {
                return (string)this["LogPath"];
            }
            set
            {
                this["LogPath"] = value;
            }
        }

        [ConfigurationProperty("LogTailLength", IsRequired = false, DefaultValue = 1024)]
        public int LogTailLength
        {
            get
            {
                return (int)this["LogTailLength"];
            }
            set
            {
                this["LogTailLength"] = value;
            }
        }
    }
}
