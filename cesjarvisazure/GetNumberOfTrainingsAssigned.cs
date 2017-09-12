using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace cesjarvisazure
{
    public static class GetNumberOfTrainingsAssigned
    {
        public static async Task<ApiAiResponse> GetTrainingMetrics(ITraceWriter log, int userId, string bearerToken, string sessionIdToken)
        {
            //Get URL to execute
            var trainingMetricsUrl = TranscriptAPI.GetTrainingMetricsURL(userId);

            //Execute URL and return results
            string trainingMetrics = string.Empty;

            ApiAiResponse response = new ApiAiResponse();
            string responseText;
            try
            {
                JObject trainingMetricsJobject = JObject.Parse(RequestHelper.ExecuteUrl(trainingMetricsUrl, bearerToken, sessionIdToken));
                JToken trainingMetricsJobjectData = trainingMetricsJobject["data"].FirstOrDefault();
                string assignedCount = trainingMetricsJobjectData["assignedCount"].ToString();
                string pastDueCount = trainingMetricsJobjectData["pastDueCount"].ToString();
                string dueSoonCount = trainingMetricsJobjectData["dueSoonCount"].ToString();

                responseText = $"You have {assignedCount} assigned trainings. {pastDueCount} are past due and {dueSoonCount} trainings are due soon.";
            }
            catch (Exception ex)
            {
                responseText = "Something went wrong. Please try again.";
            }

            response.displayText = responseText;
            response.speech = responseText;
            return response;

        }

        public static async Task<ApiAiResponse> SearchName(ITraceWriter log, string searchTerms, string bearerToken, string sessionIdToken)
        {
            //Get URL to execute
            var searchUrl = TranscriptAPI.SearchName(searchTerms);

            //Execute URL and return results
            string trainingMetrics = string.Empty;

            ApiAiResponse response = new ApiAiResponse();
            string responseText;
            try
            {
                JObject searchresultobject = JObject.Parse(RequestHelper.ExecuteUrl(searchUrl, bearerToken, sessionIdToken));
                JToken result = searchresultobject["data"].FirstOrDefault();
                string personName = result[0]["FirstName"].ToString() + " " + result[0]["LastName"].ToString();
                string personPhone = result[0]["PhoneWork"].ToString();

                responseText = $"You have {personName} results.";
            }
            catch (Exception ex)
            {
                responseText = "Something went wrong. Please try again.";
            }

            response.displayText = responseText;
            response.speech = responseText;
            return response;
        }

        public static async Task<ApiAiResponse> CreatePosting(ITraceWriter log, int requisitionId, int careerSiteId, string bearerToken, string sessionIdToken)
        {
            //Get URL to execute
            var postingUrl = TranscriptAPI.PostingURL(requisitionId);

            //Execute URL and return results
            string jsonStuff = JsonConvert.SerializeObject(new[] { new { careerSiteId = careerSiteId, startDate = DateTime.Now, endDate = DateTime.Now.AddDays(7), isDefault = true } });

            ApiAiResponse response = new ApiAiResponse();
            string responseText;
            try
            {
                JObject postingobject = JObject.Parse(RequestHelper.ExecuteUrl(postingUrl, bearerToken, sessionIdToken, jsonStuff, HttpMethod.Put.Method));
                JToken result = postingobject["data"].FirstOrDefault();
                
                responseText = $"Done.";
            }
            catch (Exception ex)
            {
                responseText = "Something went wrong. Please try again.";
            }

            response.displayText = responseText;
            response.speech = responseText;
            return response;
        }

        public static async Task<ApiAiResponse> RemovePosting(ITraceWriter log, int requisitionId, string bearerToken, string sessionIdToken)
        {
            //Get URL to execute
            var postingUrl = TranscriptAPI.PostingURL(requisitionId);

            //Execute URL and return results
            string trainingMetrics = string.Empty;

            ApiAiResponse response = new ApiAiResponse();
            string responseText;
            try
            {
                JObject postingobject = JObject.Parse(RequestHelper.ExecuteUrl(postingUrl, bearerToken, "[]", HttpMethod.Put.Method));
                JToken result = postingobject["data"].FirstOrDefault();
                
                responseText = $"Done.";
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