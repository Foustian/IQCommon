using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace IQLog.Configuration
{
    public class ProcessInfoSection : ConfigurationSection
    {
        [ConfigurationProperty("ProcessType", IsRequired = true)]
        public short ProcessType
        {
            get
            {
                return (short)this["ProcessType"];
            }
            set
            {
                this["ProcessType"] = value;
            }
        }
    }
}
