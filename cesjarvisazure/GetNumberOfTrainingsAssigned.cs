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
        //Career site url https://la4prdsl1.csod.com/ats/careersite/jobdetails.aspx?site=44&c=la4prdsl1&id=609

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
                string feedback = await RequestHelper.ExecuteUrl(trainingMetricsUrl, bearerToken, sessionIdToken);
                JObject trainingMetricsJobject = JObject.Parse(feedback);
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
                string feedback = await RequestHelper.ExecuteUrl(searchUrl, bearerToken, sessionIdToken);
                JObject searchresultobject = JObject.Parse(feedback);
                JToken result = searchresultobject["data"].FirstOrDefault();
                string personName = result["FirstName"].ToString() + " " + result["LastName"].ToString();
                string personPhone = result["PhoneWork"].ToString();
                string managerName = result["ManagerName"].ToString();
                string title = result["Title"].ToString();
                responseText = $"{personName} is {title} in {searchTerms}.";
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
                string feedback = await RequestHelper.ExecuteUrl(postingUrl, bearerToken, sessionIdToken, jsonStuff, HttpMethod.Put.Method);
                JObject postingobject = JObject.Parse(feedback);
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
                string feedback = await RequestHelper.ExecuteUrl(postingUrl, bearerToken, "[]", HttpMethod.Put.Method);
                JObject postingobject = JObject.Parse(feedback);
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

        public static async Task<ApiAiResponse> GetApplicantYesterday(ITraceWriter log, string requisitionId, string bearerToken, string sessionIdToken)
        {
            //Get URL to execute
            var requisitionUrl = TranscriptAPI.GetRequisitionDetailsURL(requisitionId);

            //Execute URL and return results
            string trainingMetrics = string.Empty;

            ApiAiResponse response = new ApiAiResponse();
            string responseText;
            try
            {
                string feedback = await RequestHelper.ExecuteUrl(requisitionUrl, bearerToken, sessionIdToken);
                JObject postingobject = JObject.Parse(feedback);
                JToken result = postingobject["data"].FirstOrDefault();
                string numberOfApplicants = result["items"].FirstOrDefault()["fields"]["applicantCount"].ToString();
                string newSubmissionCount = result["items"].FirstOrDefault()["fields"]["newSubmissionCount"].ToString();

                responseText = $"You have {numberOfApplicants} applications.";
            }
            catch (Exception ex)
            {
                responseText = "Something went wrong. Please try again.";
            }

            response.displayText = responseText;
            response.speech = responseText;
            return response;
        }

        public static async Task<ApiAiResponse> GetTopApplicants(ITraceWriter log, int requisitionId, string bearerToken, string sessionIdToken)
        {
            //Get URL to execute
            var requisitionUrl = TranscriptAPI.GetCandidateInRequisitionURL(requisitionId, CandidateType.Candidate, 1);

            //Execute URL and return results
            string trainingMetrics = string.Empty;

            ApiAiResponse response = new ApiAiResponse();
            string responseText;
            try
            {
                string feedback = await RequestHelper.ExecuteUrl(requisitionUrl, bearerToken, sessionIdToken);
                JObject postingobject = JObject.Parse(feedback);
                string numberOfRecords = postingobject["totalRecords"].ToString();
                JToken result = postingobject["data"].FirstOrDefault();
                JToken topApplicants = result["items"].FirstOrDefault();
                string topApplicantName = topApplicants["fields"]["name"].ToString();

                responseText = $"{topApplicantName} is top applicants.";
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