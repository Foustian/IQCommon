using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQSearch.Sentiment.Data;
using System.Data;
using IQLog;

namespace IQSearch.Sentiment.Model
{
    internal class IQSentimentWordModel : IQMediaGroupDataLayer
    {
        public List<IQSentimentWord> GetSentimentWordsByClientGuid(Guid p_ClientGuid)
        {
            try
            {
                List<IQSentimentWord> _ListOfIQSentiments = new List<IQSentimentWord>();
                List<DataType> _ListOfDataType = new List<DataType>();
                _ListOfDataType.Add(new DataType("@ClientGuid", DbType.Guid, p_ClientGuid, ParameterDirection.Input));
                using (IDataReader _IDataReader = GetDataReader("usp_IQ_Sentiments_SelectByClientGuid", _ListOfDataType))
                {
                    _ListOfIQSentiments = FillIQ_Sentiment(_IDataReader);
                }
                if (_ListOfIQSentiments == null)
                {
                    IQLogger.Debug("Sentiment list is null");
                }
                else if (_ListOfIQSentiments.Count <= 0)
                {
                    IQLogger.Debug("Sentiment list is empty");
                }
                else
                {
                    IQLogger.Debug("Sentiment list contains " + _ListOfIQSentiments.Count + " items");
                    /*StringBuilder builder = new StringBuilder();
                    builder.Append("Sentiment list: ");
                    foreach (var sentiment in _ListOfIQSentiments)
                    {
                        builder.AppendFormat("{{{0}, {1}, {2}}}, ", sentiment.Category, sentiment.Word, sentiment.Value);
                    }
                    IQLogger.Submit(1, builder.ToString());*/
                }
                return _ListOfIQSentiments;
            }
            catch (Exception ex)
            {
                IQLogger.BuildEvent(101)
                    .SetValue("storedProcedure", "usp_IQ_Sentiments_SelectByClientGuid")
                    .SetException(ex)
                    .Submit();
                throw;
            }
        }

        public List<IQSentimentWord> FillIQ_Sentiment(IDataReader _IDataReader)
        {
            try
            {
                List<IQSentimentWord> _ListOfIQSentiments = new List<IQSentimentWord>();

                while (_IDataReader.Read())
                {
                    IQSentimentWord _IQ_Sentiment = new IQSentimentWord();
                    _IQ_Sentiment.Word =_IDataReader.GetString(0);
                    _IQ_Sentiment.Value = _IDataReader.GetInt32(1);
                    _ListOfIQSentiments.Add(_IQ_Sentiment);
                }

                

                return _ListOfIQSentiments;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
