using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TooSimple.Extensions
{
    public static class ApiExtension
    {
        /// <summary>
        /// Consume API Endpoint
        /// </summary>
        /// <param name="url">Complete url of endpoint</param>
        /// <param name="method">HTTP Method(i.e. GET, POST, etc</param>
        /// <param name="requestJson">Json being sent to endpoint, if applicable</param>
        /// <returns></returns>
        public static async Task<string> GetApiResponse(string url, string method, string requestJson)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = method;

            if (!string.IsNullOrWhiteSpace(requestJson))
            {
                using (var writer = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    writer.Write(requestJson);
                }
            }

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                string result = null;
                using (var reader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }

                return result;
            }
            catch(WebException ex)
            {
                string result = null;
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    using (Stream data = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(data))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }

                return result;
            }
        }
    }
}
