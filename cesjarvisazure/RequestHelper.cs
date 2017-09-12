using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Http;

namespace cesjarvisazure
{
    public static class RequestHelper
    {
        public static async Task<string> ExecuteUrl(string url, string token, string sessionId, string stuffToPost = "", string httpMethod = "GET")
        {
            string r = string.Empty;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Timeout = 30000;
            webRequest.Method = httpMethod;
            webRequest.CookieContainer = new CookieContainer();
            webRequest.CookieContainer.Add(new Cookie("ASP.NET_SessionId", sessionId) { Domain = new Uri(url).Host });
            webRequest.AllowAutoRedirect = false;
            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13 ( .NET CLR 3.5.30729; .NET4.0E)";
            webRequest.Headers.Add("Authorization", token);

            if (httpMethod != HttpMethod.Get.Method)
            {
                using (StreamWriter requestStream = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestStream.Write(stuffToPost);
                }
            }

            WebResponse webResponse = null;
            try
            {
                webResponse = await webRequest.GetResponseAsync();
                using (Stream responseStream = webResponse.GetResponseStream())
                using (StreamReader sReader = new StreamReader(responseStream))
                {
                    r = sReader.ReadToEnd();
                }

            }
            catch (Exception e)
            {

            }
            finally
            {
                if (webResponse != null)
                    webResponse.Close();
            }

            return r;
        }
    }
}
