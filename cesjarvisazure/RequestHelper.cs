using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace cesjarvisazure
{
    public static class RequestHelper
    {
        public static string ExecuteUrl(string url, string token, string sessionId)
        {
            string r = string.Empty;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Timeout = 30000;
            webRequest.Method = "GET";
            webRequest.CookieContainer = new CookieContainer();
            webRequest.CookieContainer.Add(new Cookie("ASP.NET_SessionId", sessionId) { Domain = new Uri(url).Host });
            webRequest.AllowAutoRedirect = false;
            webRequest.ContentType = "application/json";
            webRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13 ( .NET CLR 3.5.30729; .NET4.0E)";
            webRequest.Headers.Add("Authorization", token);

            HttpWebResponse webResponse = null;
            try
            {
                webResponse = (HttpWebResponse)webRequest.GetResponseAsync().Result;
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
