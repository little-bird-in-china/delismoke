using MessageCenter.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MessageCenter.Processor
{
    internal class HttpClient
    {
        #region 同步请求

        #region GET

        internal static AsyncResult<T> Get<T>(string url) where T : class
        {
            return Get<T>(url, ContentTypes.Application_Json, string.Empty);
        }

        internal static AsyncResult<T> Get<T>(string url, string contentType) where T : class
        {
            return Get<T>(url, contentType, string.Empty);
        }

        internal static AsyncResult<T> Get<T>(string url, string contentType, string userAgent) where T : class
        {
            return Execute<T>(url, string.Empty, HttpMethod.GET, contentType, userAgent);
        }

        #endregion

        #region POST

        internal static AsyncResult<T> Post<T>(string url, string data) where T : class
        {
            return Post<T>(url, data, ContentTypes.Application_Json);
        }
        internal static AsyncResult<T> Post<T>(string url, string data, string contentType) where T : class
        {
            return Post<T>(url, data, contentType, string.Empty);
        }
        internal static AsyncResult<T> Post<T>(string url, string data, string contentType, string userAgent) where T : class
        {
            return Execute<T>(url, data, HttpMethod.POST, contentType, userAgent);
        }

        #endregion


        #endregion

        #region 异步请求

        #region GET

        internal static void Get<T>(string url, Action<AsyncResult<T>> callback) where T : class
        {
            Get<T>(url, ContentTypes.Application_Json, callback);
        }

        internal static void Get<T>(string url, string contentType, Action<AsyncResult<T>> callback) where T : class
        {
            Get<T>(url, contentType, string.Empty, callback);
        }

        internal static void Get<T>(string url, string contentType, string userAgent, Action<AsyncResult<T>> callback) where T : class
        {
            ExecuteAsync<T>(url, string.Empty, HttpMethod.GET, contentType, userAgent, callback);
        }

        #endregion

        #region POST

        internal static void Post<T>(string url, string data, Action<AsyncResult<T>> callback) where T : class
        {
            Post<T>(url, data, ContentTypes.Application_Json, callback);
        }
        internal static void Post<T>(string url, string data, string contentType, Action<AsyncResult<T>> callback) where T : class
        {
            Post<T>(url, data, contentType, string.Empty, callback);
        }
        internal static void Post<T>(string url, string data, string contentType, string userAgent, Action<AsyncResult<T>> callback) where T : class
        {
            ExecuteAsync<T>(url, data, HttpMethod.POST, contentType, userAgent, callback);
        }

        #endregion

        #endregion

        /// <summary>
        /// 同步请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <param name="contentType"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        private static AsyncResult<T> Execute<T>(string url, string data, HttpMethod method, string contentType, string userAgent) where T : class
        {
            AsyncResult<T> result = new AsyncResult<T>();
            if (string.IsNullOrEmpty(url))
            {
                result.Error = new Exception("请求地址不能为空");
                return result;
            }

            Uri uri = new Uri(url);

            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            if (request == null)
            {
                result.Error = new Exception("创建请求失败");
                return result;
            }

            request.Method = method.ToString();
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            request.ContentType = contentType;

            if (!string.IsNullOrEmpty(data))
            {
                byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(data);
                request.ContentLength = bytes.Length;
                using (Stream writer = request.GetRequestStream())
                {
                    writer.Write(bytes, 0, bytes.Length);
                }
            }
            WebResponse response = null;
            try
            {
                response = request.GetResponse();
                BuildResult<T>(response, result);
            }
            catch (WebException ex)
            {

                response = ex.Response;
                BuildResult<T>(response, result);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return result;
        }


        /// <summary>
        /// 异步请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <param name="contentType"></param>
        /// <param name="userAgent"></param>
        /// <param name="callback"></param>
        private static void ExecuteAsync<T>(string url, string data, HttpMethod method, string contentType, string userAgent, Action<AsyncResult<T>> callback) where T : class
        {
            if (string.IsNullOrEmpty(url))
                return;
            Uri uri = new Uri(url);

            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            if (request == null)
                return;

            request.Method = method.ToString();
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            request.ContentType = contentType;

            if (!string.IsNullOrEmpty(data))
            {
                byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(data);
                request.ContentLength = bytes.Length;
                request.BeginGetRequestStream((ar) =>
                {
                    using (Stream writer = request.EndGetRequestStream(ar))
                    {
                        writer.Write(bytes, 0, bytes.Length);
                    }

                    BeginGetResponse<T>(callback, request);
                }, null);
            }
            else
            {
                BeginGetResponse<T>(callback, request);
            }
        }

        private static void BeginGetResponse<T>(Action<AsyncResult<T>> callback, HttpWebRequest requst) where T : class
        {
            if (requst == null)
                return;

            requst.BeginGetResponse((ar) =>
            {
                var result = new AsyncResult<T>();
                WebResponse response = null;
                try
                {
                    response = requst.EndGetResponse(ar);
                    BuildResult<T>(response, result);
                    if (callback != null)
                    {
                        callback(result);
                    }
                }
                catch (WebException ex)
                {
                    result.Error = ex;
                    //TODO 记录日志
                    if (callback != null)
                    {
                        response = ex.Response;
                        BuildResult<T>(response, result);
                        callback(result);
                    }
                }
                finally
                {
                    if (response != null)
                        response.Close();
                }

            }, null);
        }

        /// <summary>
        /// 构造Result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static AsyncResult<T> BuildResult<T>(WebResponse response, AsyncResult<T> result) where T : class
        {
            if (response != null)
            {
                if (response.ContentLength > 0)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string str = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (response.ContentType.Contains(ContentTypes.Application_Json))
                            {
                                result.Result = JsonConvert.DeserializeObject<T>(str);
                            }
                            if (response.ContentType.Contains(ContentTypes.Application_Xml))
                            {
                                result.Result = DeserializeXmlStr<T>(str);
                            }
                        }
                    }
                }

                response.Close();
                response = null;
            }

            return result;
        }

        #region Serialize

        /// <summary>
        /// XML序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string SerializeXML<T>(T data)
        {


            return Xml.XmlSerializer.SerializeToString<T>(data); 
            StringBuilder str = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(str);
            XmlSerializer serialize = new XmlSerializer(typeof(T));
            serialize.Serialize(writer, data);
            writer.Close();

            return str.ToString();
        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strXml">xml字符串</param>
        /// <returns></returns>
        public static T DeserializeXmlStr<T>(string strXml)
            where T : class
        {
            return Xml.XmlSerializer.DeserializeFromString<T>(strXml);
            if (!string.IsNullOrEmpty(strXml))
            {
                StringReader strReader = new StringReader(strXml);
                XmlReader xmlReader = XmlReader.Create(strReader);
                if (xmlReader != null)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    T data = serializer.Deserialize(xmlReader) as T;
                    xmlReader.Close();
                    return data;
                }
            }

            return null;
        }

        #endregion



    }
}
