using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IQCommon.Model;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.IO;
using System.Xml.Linq;

namespace IQCommon
{
    public static class CommonFunctions
    {
        public static string ConnString { get; set; }

        public static string AesKeyFeedsRadioPlayer = "632B783177697669622F333041426133";
        public static string AesIVFeedsRadioPlayer = "3955456236643867";

        public static List<IQ_MediaTypeModel> GetMediaTypes(Guid customerGuid)
        {
            try
            {
                List<IQ_MediaTypeModel> mediaTypes = new List<IQ_MediaTypeModel>();

                using (var conn = new SqlConnection(ConnString))
                {
                    conn.Open();

                    using (var cmd = conn.GetCommand("usp_common_IQ_MediaTypes_SelectWithRoles", CommandType.StoredProcedure))
                    {
                        cmd.Parameters.Add("@CustomerGuid", SqlDbType.UniqueIdentifier).Value = customerGuid;

                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            IQ_MediaTypeModel mediaType = new IQ_MediaTypeModel();
                            mediaType.MediaType = Convert.ToString(reader["MediaType"]);
                            mediaType.SubMediaType = Convert.ToString(reader["SubMediaType"]);
                            mediaType.DisplayName = Convert.ToString(reader["DisplayName"]);
                            mediaType.TypeLevel = Convert.ToInt16(reader["TypeLevel"]);
                            mediaType.HasSubMediaTypes = Convert.ToBoolean(reader["HasSubMediaTypes"]);
                            mediaType.DataModelType = Convert.ToString(reader["DataModelType"]);
                            mediaType.AnalyticsDataType = Convert.ToString(reader["AnalyticsDataType"]);
                            mediaType.DiscChartSearchMethod = Convert.ToString(reader["DiscChartSearchMethod"]);
                            mediaType.DiscResultsSearchMethod = Convert.ToString(reader["DiscResultsSearchMethod"]);
                            mediaType.DiscRptGenSearchMethod = Convert.ToString(reader["DiscRptGenSearchMethod"]);
                            mediaType.DiscExportSearchMethod = Convert.ToString(reader["DiscExportSearchMethod"]);
                            mediaType.FeedsResultView = Convert.ToString(reader["FeedsResultView"]);
                            mediaType.FeedsChildResultView = Convert.ToString(reader["FeedsChildResultView"]);
                            mediaType.DiscoveryResultView = Convert.ToString(reader["DiscoveryResultView"]);
                            mediaType.MediaIconPath = Convert.ToString(reader["MediaIconPath"]);
                            mediaType.EmailMediaIconPath = Convert.ToString(reader["EmailMediaIconPath"]);
                            mediaType.UseAudience = Convert.ToBoolean(reader["UseAudience"]);
                            mediaType.UseMediaValue = Convert.ToBoolean(reader["UseMediaValue"]);
                            mediaType.SortOrder = Convert.ToInt16(reader["SortOrder"]);
                            mediaType.IsActiveDiscovery = Convert.ToBoolean(reader["IsActiveDiscovery"]);
                            mediaType.IsArchiveOnly = Convert.ToBoolean(reader["IsArchiveOnly"]);
                            mediaType.UseHighlightingText = Convert.ToBoolean(reader["UseHighlightingText"]);
                            mediaType.HasAccess = Convert.ToBoolean(reader["HasAccess"]);
                            mediaType.RequireNielsenAccess = Convert.ToBoolean(reader["RequireNielsenAccess"]);
                            mediaType.RequireCompeteAccess = Convert.ToBoolean(reader["RequireCompeteAccess"]);
                            mediaType.AgentNodeName = Convert.ToString(reader["AgentNodeName"]);
                            mediaType.SourceTypes = Convert.ToString(reader["SourceTypes"]).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            mediaType.SolrMediaType = Convert.ToString(reader["SolrMediaType"]);
                            mediaType.ExternalPlayerUrlSvc = Convert.ToString(reader["ExternalPlayerUrlSvc"]);
                            if (!reader.IsDBNull(reader.GetOrdinal("AgentType")))
                            {
                                mediaType.AgentType = (AgentType)Convert.ToInt16(reader["AgentType"]);
                            }                           

                            // Can change to xml to support xquery in sql
                            var dashboardJsonData = Convert.ToString(reader["DashboardData"]);

                            if (!string.IsNullOrEmpty(dashboardJsonData))
                            {
                                mediaType.DashboardData = new DashboardData();

                                mediaType.DashboardData = (DashboardData)Newtonsoft.Json.JsonConvert.DeserializeObject(dashboardJsonData, mediaType.DashboardData.GetType());

                                if (mediaType.DashboardData.ChartTypes == null)
                                {
                                    mediaType.DashboardData.ChartTypes = new List<DataChart>();
                                }

                                if (mediaType.DashboardData.DataLists == null)
                                {
                                    mediaType.DashboardData.DataLists = new List<DataList>();
                                }

                                if (mediaType.DashboardData.ArchiveDataLists == null)
                                {
                                    mediaType.DashboardData.ArchiveDataLists = new List<DataList>();
                                }
                            }

                            mediaTypes.Add(mediaType);
                        }
                    }
                }

                return mediaTypes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // When calling FeedsSearch, used to filter out IQAgents and individual Feeds records that have been queued for deletion but not yet processed in solr
        public static Dictionary<string, List<string>> GetQueuedDeleteMediaResults(Guid clientGuid)
        {
            try
            {
                Dictionary<string, List<string>> dictResult = new Dictionary<string, List<string>>();

                using (var conn = new SqlConnection(ConnString))
                {
                    conn.Open();

                    using (var cmd = conn.GetCommand("usp_common_IQAgent_MediaResults_SelectInactive", CommandType.StoredProcedure))
                    {
                        cmd.Parameters.Add("@ClientGUID", SqlDbType.UniqueIdentifier).Value = clientGuid;

                        var reader = cmd.ExecuteReader();

                        // Deleted IQSeqIDs
                        List<string> ids = new List<string>();
                        while (reader.Read())
                        {
                            ids.Add(Convert.ToString(reader["ID"]));
                        }
                        dictResult.Add("ExcludeIDs", ids);

                        // Deleted IQAgent IDs
                        if (reader.NextResult())
                        {
                            List<string> agentIDs = new List<string>();
                            while (reader.Read())
                            {
                                agentIDs.Add(Convert.ToString(reader["SearchRequestID"]));
                            }
                            dictResult.Add("ExcludeSearchRequestIDs", agentIDs);
                        }
                    }
                }

                return dictResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // When calling FeedsSearch, used to filter out records that have been marked as read/unread but not yet processed in solr
        public static Dictionary<string, bool> GetQueuedIsRead(Guid clientGuid)
        {
            try
            {
                Dictionary<string, bool> dictResult = new Dictionary<string, bool>();

                using (var conn = new SqlConnection(ConnString))
                {
                    conn.Open();

                    using (var cmd = conn.GetCommand("usp_common_IQAgent_MediaResults_SelectQueuedIsRead", CommandType.StoredProcedure))
                    {
                        cmd.Parameters.Add("@ClientGUID", SqlDbType.UniqueIdentifier).Value = clientGuid;

                        var reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            dictResult.Add(Convert.ToString(reader["ID"]), Convert.ToBoolean(reader["IsRead"]));
                        }
                    }
                }

                return dictResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static int UpdateIsRead(Guid clientGuid, List<string> mediaIDs, bool isRead)
        {
            try
            {
                string strMediaIDs = null;
                if (mediaIDs != null && mediaIDs.Count > 0)
                {
                    XDocument xdoc = new XDocument(new XElement("list",
                                                 from ele in mediaIDs
                                                 select new XElement("item", new XAttribute("id", ele))
                                                         ));
                    strMediaIDs = xdoc.ToString();
                }

                using (var conn = new SqlConnection(ConnString))
                {
                    conn.Open();

                    using (var cmd = conn.GetCommand("usp_common_IQAgent_MediaResults_UpdateIsRead", CommandType.StoredProcedure))
                    {
                        cmd.Parameters.Add("@ClientGUID", SqlDbType.UniqueIdentifier).Value = clientGuid;
                        cmd.Parameters.Add("@MediaIDXml", SqlDbType.Xml).Value = strMediaIDs;
                        cmd.Parameters.Add("@IsRead", SqlDbType.Bit).Value = isRead;

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string EncryptStringAES(string rawString, string key, string iv)
        {
            byte[] encrypted;
            UTF8Encoding encoding = new UTF8Encoding();

            // Create an AesManaged object
            // with the specified key and IV.
            using (AesManaged aesManaged = new AesManaged())
            {
                aesManaged.Key = encoding.GetBytes(key);
                aesManaged.IV = encoding.GetBytes(iv);

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesManaged.CreateEncryptor(aesManaged.Key, aesManaged.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(rawString);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted);
        }

        public static IEnumerable<IQ_MediaTypeModel> GetAccessibleSubMediaType(List<IQ_MediaTypeModel> p_MediaTypeList)
        {
            var subMediaTypeList = p_MediaTypeList.Where(mt => mt.TypeLevel == 2 && mt.HasAccess && !mt.IsArchiveOnly);

            return subMediaTypeList;
        }

        public static IEnumerable<IQ_MediaTypeModel> GetAccessibleSubMediaType(List<string> p_SubMediaTypeList, List<IQ_MediaTypeModel> p_MediaTypeList)
        {
            var subMediaTypeList = p_MediaTypeList.Where(mt => mt.TypeLevel == 2 && mt.HasAccess && p_SubMediaTypeList.Contains(mt.SubMediaType));

            return subMediaTypeList;
        }
    }
}
