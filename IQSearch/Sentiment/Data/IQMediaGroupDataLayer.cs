using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using System.Data;
using System.Xml;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using System.Data.Common;
using System.Data.SqlClient;

namespace IQSearch.Sentiment.Data
{
    internal class IQMediaGroupDataLayer
    {
        public enum ConnectionStringKeys
        {
            IQMediaGroupConnectionString = 0,
            IQMediaGroupQAConnection = 1,
            IQMediaGroupProductionConnection = 2,
            IQMediaGroupDeveloperConnection = 3
        }

        private  string _CONNECTION_STRING_KEY = ConnectionStringKeys.IQMediaGroupConnectionString.ToString();
        

        public  string CONNECTION_STRING_KEY
        {
            get
            {
                //_CONNECTION_STRING_KEY = HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToString();
                return _CONNECTION_STRING_KEY; 
            }
            set { _CONNECTION_STRING_KEY = value; }
        }

        public IDataReader GetDataReaderByStatement(string sqlCommand)
        {
            Database db = DatabaseFactory.CreateDatabase(CONNECTION_STRING_KEY);
            DbCommand dbCommand = db.GetSqlStringCommand(sqlCommand);


            // The ExecuteReader call will request the connection to be closed upon
            // the closing of the DataReader. The DataReader will be closed 
            // automatically when it is disposed.
            IDataReader dataReader = db.ExecuteReader(dbCommand);

            return dataReader;
        }


        public string ExecuteNonQuery(string ProcedureName, List<DataType> oListOfDataType)
        {

            Database oDatabase = DatabaseFactory.CreateDatabase(CONNECTION_STRING_KEY);
            DbCommand oDbCommand = oDatabase.GetStoredProcCommand(ProcedureName);

            Int32 returnValue = 0;

            string sOutPramName = string.Empty;
            string _ReturnValue = string.Empty;

            foreach (DataType oDataType in oListOfDataType)
            {
                if (oDataType != null)
                {
                    if (oDataType.Direction == ParameterDirection.Input)
                    {
                        oDatabase.AddInParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, oDataType.Value);
                    }

                    if (oDataType.Direction == ParameterDirection.Output || oDataType.Direction == ParameterDirection.InputOutput)
                    {
                        oDatabase.AddOutParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, returnValue);
                        sOutPramName = oDataType.ParameterName;
                    }
                }

            }

            _ReturnValue = Convert.ToString(oDatabase.ExecuteNonQuery(oDbCommand));

            if (!string.IsNullOrEmpty(sOutPramName))
            {
                _ReturnValue = Convert.ToString(oDatabase.GetParameterValue(oDbCommand, sOutPramName));

            }

            return _ReturnValue;
        }

        //public string ExecuteNonQueryWithSQLDatType(string ProcedureName, List<SQLDataType> oListOfDataType)
        //{

        //    Database oDatabase = DatabaseFactory.CreateDatabase(CONNECTION_STRING_KEY);
        //    DbCommand oDbCommand = oDatabase.GetStoredProcCommand(ProcedureName);

        //    Int32 returnValue = 0;

        //    string sOutPramName = string.Empty;
        //    string _ReturnValue = string.Empty;

        //    foreach (SQLDataType oDataType in oListOfDataType)
        //    {
        //        if (oDataType != null)
        //        {
        //            if (oDataType.Direction == ParameterDirection.Input)
        //            {
        //                oDatabase.AddInParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, oDataType.Value);
        //            }

        //            if (oDataType.Direction == ParameterDirection.Output || oDataType.Direction == ParameterDirection.InputOutput)
        //            {
        //                oDatabase.AddOutParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, returnValue);
        //                sOutPramName = oDataType.ParameterName;
        //            }
        //        }

        //    }

        //    _ReturnValue = Convert.ToString(oDatabase.ExecuteNonQuery(oDbCommand));

        //    if (!string.IsNullOrEmpty(sOutPramName))
        //    {
        //        _ReturnValue = Convert.ToString(oDatabase.GetParameterValue(oDbCommand, sOutPramName));

        //    }

        //    return _ReturnValue;
        //}

        public List<string> ExecuteNonQuery(string ProcedureName, List<DataType> oListOfDataType, bool Status)
        {

            Database oDatabase = DatabaseFactory.CreateDatabase(CONNECTION_STRING_KEY);

            DbCommand oDbCommand = oDatabase.GetStoredProcCommand(ProcedureName);

            Int32 returnValue = 0;

            string sOutPramName = string.Empty;
            string _ReturnValue = string.Empty;

            List<string> _ListOfReturn = new List<string>();

            foreach (DataType oDataType in oListOfDataType)
            {
                if (oDataType != null)
                {
                    if (oDataType.Direction == ParameterDirection.Input)
                    {
                        oDatabase.AddInParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, oDataType.Value);
                    }

                    if (oDataType.Direction == ParameterDirection.Output)
                    {
                        oDatabase.AddOutParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, returnValue);
                        sOutPramName = oDataType.ParameterName;
                    }
                }

            }

            _ReturnValue = Convert.ToString(oDatabase.ExecuteNonQuery(oDbCommand));

            if (!string.IsNullOrEmpty(sOutPramName))
            {
                foreach (DataType oDataType in oListOfDataType)
                {
                    if (oDataType.Direction == ParameterDirection.Output)
                    {
                        _ReturnValue = Convert.ToString(oDatabase.GetParameterValue(oDbCommand, oDataType.ParameterName));
                        _ListOfReturn.Add(_ReturnValue);
                    }
                }
            }

            return _ListOfReturn;
        }

        public string ExecuteNonQuery(string ProcedureName, List<DataType> oListOfDataType, out Dictionary<string, string> p_Outputparams)
        {

            Database oDatabase = DatabaseFactory.CreateDatabase(CONNECTION_STRING_KEY);

            DbCommand oDbCommand = oDatabase.GetStoredProcCommand(ProcedureName);

            Int32 returnValue = 65535;

            string sOutPramName = string.Empty;
            string _ReturnValue = string.Empty;


            p_Outputparams = new Dictionary<string, string>();

            foreach (DataType oDataType in oListOfDataType)
            {
                if (oDataType != null)
                {
                    if (oDataType.Direction == ParameterDirection.Input)
                    {
                        oDatabase.AddInParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, oDataType.Value);
                    }

                    if (oDataType.Direction == ParameterDirection.Output)
                    {
                        oDatabase.AddOutParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, returnValue);
                        sOutPramName = oDataType.ParameterName;
                    }
                }

            }

            _ReturnValue = Convert.ToString(oDatabase.ExecuteNonQuery(oDbCommand));

            if (!string.IsNullOrEmpty(sOutPramName))
            {
                string _OutputParamValue = string.Empty;

                foreach (DataType oDataType in oListOfDataType)
                {
                    if (oDataType.Direction == ParameterDirection.Output)
                    {
                        _OutputParamValue = Convert.ToString(oDatabase.GetParameterValue(oDbCommand, oDataType.ParameterName));
                        p_Outputparams.Add(oDataType.ParameterName, _OutputParamValue);
                    }
                }
            }

            return _ReturnValue;
        }

        public DataSet GetDataSet(string sProcedureName, List<DataType> oListOfDataType)
        {
            DataSet oDataSet = null;
            try
            {

                Database oDatabase = DatabaseFactory.CreateDatabase(CONNECTION_STRING_KEY);
                DbCommand oDbCommand = oDatabase.GetStoredProcCommand(sProcedureName);
                oDbCommand.CommandTimeout = 0;

                foreach (DataType oDataType in oListOfDataType)
                {
                    oDatabase.AddInParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, oDataType.Value);
                }

                oDataSet = oDatabase.ExecuteDataSet(oDbCommand);

            }
            catch (Exception oException)
            {
                throw oException;
            }
            return oDataSet;

        }

        public DataSet GetDataSetWithOutParam(string sProcedureName, List<DataType> oListOfDataType,out string returnValue )
        {
            DataSet oDataSet = null;
            try
            {
                returnValue = string.Empty;

                Database oDatabase = DatabaseFactory.CreateDatabase(CONNECTION_STRING_KEY);
                DbCommand oDbCommand = oDatabase.GetStoredProcCommand(sProcedureName);
                oDbCommand.CommandTimeout = 0;

                int returnSize=0;                            

                string sOutPramName = string.Empty;

                foreach (DataType oDataType in oListOfDataType)
                {
                    if (oDataType.Direction == ParameterDirection.Input)
                    {
                        oDatabase.AddInParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, oDataType.Value);
                    }

                    if (oDataType.Direction == ParameterDirection.Output)
                    {
                        oDatabase.AddOutParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, returnSize);
                        sOutPramName = oDataType.ParameterName;
                    }
                }

                oDataSet = oDatabase.ExecuteDataSet(oDbCommand);

                if (!string.IsNullOrEmpty(sOutPramName))
                {
                    returnValue = Convert.ToString(oDatabase.GetParameterValue(oDbCommand, sOutPramName));
                }

            }
            catch (Exception oException)
            {
                throw oException;
            }
            return oDataSet;

        }

        public DataSet GetDataSetWithOutParam(string sProcedureName, List<DataType> oListOfDataType, out Dictionary<string, string> p_Outputparams)
        {
            DataSet oDataSet = null;
            try
            {
                p_Outputparams = new Dictionary<string, string>();

                Database oDatabase = DatabaseFactory.CreateDatabase(CONNECTION_STRING_KEY);
                DbCommand oDbCommand = oDatabase.GetStoredProcCommand(sProcedureName);
                oDbCommand.CommandTimeout = 0;

                int returnSize = 0;

                string sOutPramName = string.Empty;

                foreach (DataType oDataType in oListOfDataType)
                {
                    if (oDataType.Direction == ParameterDirection.Input)
                    {
                        oDatabase.AddInParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, oDataType.Value);
                    }

                    if (oDataType.Direction == ParameterDirection.Output)
                    {
                        oDatabase.AddOutParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, returnSize);
                        sOutPramName = oDataType.ParameterName;
                    }
                }

                oDataSet = oDatabase.ExecuteDataSet(oDbCommand);


                if (!string.IsNullOrEmpty(sOutPramName))
                {
                    string _OutputParamValue = string.Empty;

                    foreach (DataType oDataType in oListOfDataType)
                    {
                        if (oDataType.Direction == ParameterDirection.Output)
                        {
                            _OutputParamValue = Convert.ToString(oDatabase.GetParameterValue(oDbCommand, oDataType.ParameterName));
                            p_Outputparams.Add(oDataType.ParameterName, _OutputParamValue);
                        }
                    }
                }
               

            }
            catch (Exception oException)
            {
                throw oException;
            }
            return oDataSet;

        }

        public int ExecuteScalar(string ProcedureName, List<DataType> oListOfDataType)
        {

            Database oDatabase = DatabaseFactory.CreateDatabase(CONNECTION_STRING_KEY);
            DbCommand oDbCommand = oDatabase.GetStoredProcCommand(ProcedureName);

            int returnValue = 0;
            int iReturn = 0;

            string sOutPramName = string.Empty;


            foreach (DataType oDataType in oListOfDataType)
            {
                if (oDataType != null)
                {
                    if (oDataType.Direction == ParameterDirection.Input)
                    {
                        oDatabase.AddInParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, oDataType.Value);
                    }

                    if (oDataType.Direction == ParameterDirection.Output)
                    {
                        oDatabase.AddOutParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, returnValue);
                        sOutPramName = oDataType.ParameterName;
                    }
                }

            }

            iReturn = Convert.ToInt32(oDatabase.ExecuteScalar(oDbCommand));
            if (!string.IsNullOrEmpty(sOutPramName))
            {
                returnValue = (int)oDatabase.GetParameterValue(oDbCommand, sOutPramName);
                return returnValue;
            }

            return iReturn;
        }

        /// <summary>
        /// This function is used to retrieve last affected text from database.
        /// </summary>
        /// <param name="ProcedureName">Stored procedure name.</param>
        /// <param name="oListOfDataType">List Of DataTypes.</param>
        /// <param name="returnVal">Return last affected text.</param>
        public void ExecuteScalar(string ProcedureName, List<DataType> oListOfDataType, ref string returnVal)
        {
            try
            {
                Database oDatabase = DatabaseFactory.CreateDatabase(CONNECTION_STRING_KEY);
                DbCommand oDbCommand = oDatabase.GetStoredProcCommand(ProcedureName);

                foreach (DataType oDataType in oListOfDataType)
                {
                    if (oDataType != null)
                    {
                        if (oDataType.Direction == ParameterDirection.Input)
                        {
                            oDatabase.AddInParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, oDataType.Value);
                        }
                    }
                }
                returnVal = Convert.ToString(oDatabase.ExecuteScalar(oDbCommand));
            }
            catch (Exception oException)
            {
                throw oException;
            }
           
        }

        public IDataReader GetDataReader(string ProcedureName, List<DataType> oListOfDataType)
        {
            try
            {
                Database oDatabase = DatabaseFactory.CreateDatabase(CONNECTION_STRING_KEY);
                DbCommand oDbCommand = oDatabase.GetStoredProcCommand(ProcedureName);

                foreach (DataType oDataType in oListOfDataType)
                {
                    if (oDataType != null)
                    {
                        if (oDataType.Direction == ParameterDirection.Input)
                        {
                            oDatabase.AddInParameter(oDbCommand, oDataType.ParameterName, oDataType.dbType, oDataType.Value);
                        }
                    }
                }

                IDataReader dataReader = oDatabase.ExecuteReader(oDbCommand);
                return dataReader;

            }
            catch (Exception oException)
            {
                throw oException;
            }
        }

        
        public Database CreateDataBase()
        {
            return DatabaseFactory.CreateDatabase(CONNECTION_STRING_KEY);            
        }
        
        
    }
}
