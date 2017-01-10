using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Net;
using System.Xml;

namespace IQSearch
{
    static class RestClient
    {
        public static String getXML(String URL, List<KeyValuePair<string, string>> vars, bool IsPMGLogging, string PMGLogFileLocation, Int32? timeOutPeriod = null, string RawParam = null)
        {
            StringBuilder data = new StringBuilder();
            int c = 0;
            foreach (KeyValuePair<String, String> kvp in vars)
            {
                if (c > 0) data.Append("&");
                data.Append(kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value));
                c++;
            }

            data = data.Append(!string.IsNullOrWhiteSpace(RawParam) ? RawParam : string.Empty);

            string _URL = URL + data.ToString();

            CommonFunction.LogInfo(_URL, IsPMGLogging, PMGLogFileLocation);

            try
            {
                Uri address = new Uri(URL);
                String ret = string.Empty;

                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = timeOutPeriod == null ? 210000 : (Int32)timeOutPeriod;

                byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                request.ContentLength = byteData.Length;

                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    ret = reader.ReadToEnd();

                    if (ret != null)
                    {
                        CommonFunction.LogInfo(ret.Length > 3000 ? ret.Substring(0, 3000) : ret, IsPMGLogging, PMGLogFileLocation);
                    }
                    else
                    {
                        CommonFunction.LogInfo("Null response", IsPMGLogging, PMGLogFileLocation);
                    }

                    return ret;
                }
            }
            catch (WebException ex)
            {
                throw parseWebException(ex, _URL);
            }

        }

        public static String getXML(String URL, List<KeyValuePair<string, string>> vars, bool IsPMGLogging, string PMGLogFileLocation, out string RequestURL, string RawParam = null)
        {
            StringBuilder data = new StringBuilder();
            int c = 0;
            foreach (KeyValuePair<String, String> kvp in vars)
            {
                if (c > 0) data.Append("&");
                data.Append(kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value));
                c++;
            }

            data = data.Append(!string.IsNullOrWhiteSpace(RawParam) ? RawParam : string.Empty);

            string _URL = URL + data.ToString();

            CommonFunction.LogInfo(_URL, IsPMGLogging, PMGLogFileLocation);
            RequestURL = _URL.Remove(URL.LastIndexOf("/")) + "?" + data.ToString();

            try
            {
                Uri address = new Uri(URL);
                String ret = string.Empty;

                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 210000;

                byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                request.ContentLength = byteData.Length;

                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    ret = reader.ReadToEnd();

                    if (ret != null)
                    {
                        CommonFunction.LogInfo(ret.Length > 3000 ? ret.Substring(0, 3000) : ret, IsPMGLogging, PMGLogFileLocation);
                    }
                    else
                    {
                        CommonFunction.LogInfo("Null response", IsPMGLogging, PMGLogFileLocation);
                    }

                    return ret;
                }
            }
            catch (WebException ex)
            {
                throw parseWebException(ex, _URL);
            }

        }

        public static String getFacet(String URL, List<KeyValuePair<string, string>> vars, bool IsPMGLogging, string PMGLogFileLocation)
        {
            StringBuilder data = new StringBuilder();
            int c = 0;
            foreach (KeyValuePair<String, String> kvp in vars)
            {
                if (c > 0) data.Append("&");
                data.Append(kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value));
                c++;
            }

            string _URL = URL + data.ToString();

            CommonFunction.LogInfo(_URL, IsPMGLogging, PMGLogFileLocation);

            try
            {

                Uri address = new Uri(URL);
                String ret = string.Empty;

                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 210000;

                byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                request.ContentLength = byteData.Length;

                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    ret = reader.ReadToEnd();

                    if (ret != null)
                    {
                        CommonFunction.LogInfo(ret.Length > 3000 ? ret.Substring(0, 3000) : ret, IsPMGLogging, PMGLogFileLocation);
                    }
                    else
                    {
                        CommonFunction.LogInfo("Null response", IsPMGLogging, PMGLogFileLocation);
                    }

                    return ret;
                }
            }
            catch (WebException ex)
            {
                throw parseWebException(ex, _URL);
            }
        }

        private static Exception parseWebException(WebException ex, string url)
        {
            HttpWebResponse response = null;
           
            if (ex.Response != null)
            {
                try
                {
                    response = (HttpWebResponse)ex.Response;
                }
                catch (Exception) { }
            }
            if (response != null && response.StatusCode == HttpStatusCode.BadRequest)
            {
                string error = null;

                try
                {
                    XmlDocument xml = new XmlDocument();
                    var reader = new XmlTextReader(ex.Response.GetResponseStream());
                    xml.Load(reader);

                    var msgNode = xml.SelectSingleNode("/response/lst[@name=\"error\"]/str[@name=\"msg\"]");

                    if (msgNode != null)
                    {
                        error = msgNode.InnerText;
                    }
                }
                catch (Exception) { }

                return new BadRequestException(error, url, ex);
            }

            return ex;
        }
    }
}
