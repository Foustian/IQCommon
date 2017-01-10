using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace IQSearch.Sentiment.Data
{
    public class DataType
    {
        public string ParameterName { get; set; }

        public DbType dbType { get; set; }

        public object Value { get; set; }

        public ParameterDirection Direction { get; set; }

        public DataType()
        {

        }

        public DataType(string ParameterName, DbType dbType, object Value, ParameterDirection Direction)
        {
            this.ParameterName = ParameterName;
            this.dbType = dbType;
            this.Value = Value;
            this.Direction = Direction;
        }
    }
}
