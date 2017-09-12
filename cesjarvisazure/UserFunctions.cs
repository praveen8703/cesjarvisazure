using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Tracing;

namespace cesjarvisazure
{
    public static class UserFunctions
    {
        public static async Task<ApiAiResponse> SearchName(TraceWriter log, string searchTerms, string bearerToken, string sessionIdToken)
        {
            ApiAiResponse response = new ApiAiResponse();
            string responseText;
            if (string.IsNullOrEmpty(searchTerms))
            {
                responseText = "Search term is empty. Please try again.";
                return response;
            }
            //Get URL to execute
            var searchUrl = TranscriptAPI.SearchName(searchTerms);

            //Execute URL and return results
            string trainingMetrics = string.Empty;
            
            try
            {
                JObject searchresultobject = JObject.Parse(RequestHelper.ExecuteUrl(searchUrl, bearerToken, sessionIdToken));
                JToken result = searchresultobject["data"].FirstOrDefault();
                if (result.Any())
                {
                    string personName = result["FirstName"].ToString() + " " + result["LastName"].ToString();
                    string personPhone = result["PhoneWork"].ToString();
                    string managerName = result["ManagerName"].ToString();
                    string title = result["Title"].ToString();
                    string emailAddress = result["EmailAddress"].ToString();
                    responseText = $"Top result found is {personName} who is a {title}. Their email address is {emailAddress}";
                }
                else
                {
                    responseText = "Could not find any results with the provided search terms";
                }
            }
            catch (Exception ex)
            {
                responseText = "Something went wrong. Please try again.";
            }

            response.displayText = responseText;
            response.speech = responseText;
            return response;
        }

    }
}
