using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace cesjarvisazure
{
    public static class ATSFunctions
    {
        //Career site url https://la4prdsl1.csod.com/ats/careersite/jobdetails.aspx?site=44&c=la4prdsl1&id=609

        public static async Task<ApiAiResponse> CreatePosting(TraceWriter log, int requisitionId, int careerSiteId, string bearerToken, string sessionIdToken)
        {
            //Get URL to execute
            var postingUrl = TranscriptAPI.PostingURL(requisitionId);

            //Execute URL and return results
            string jsonStuff = JsonConvert.SerializeObject(new[] { new { careerSiteId = careerSiteId, startDate = DateTime.Now, endDate = DateTime.Now.AddDays(7), isDefault = true } });

            ApiAiResponse response = new ApiAiResponse();
            string responseText;
            try
            {
                JObject postingobject = JObject.Parse(await RequestHelper.ExecuteUrl(postingUrl, bearerToken, sessionIdToken, jsonStuff, HttpMethod.Put.Method));
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

        public static async Task<ApiAiResponse> RemovePosting(TraceWriter log, int requisitionId, string bearerToken, string sessionIdToken)
        {
            //Get URL to execute
            var postingUrl = TranscriptAPI.PostingURL(requisitionId);

            //Execute URL and return results
            string trainingMetrics = string.Empty;

            ApiAiResponse response = new ApiAiResponse();
            string responseText;
            try
            {
                JObject postingobject = JObject.Parse(await RequestHelper.ExecuteUrl(postingUrl, bearerToken, "[]", HttpMethod.Put.Method));
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

        public static async Task<ApiAiResponse> GetApplicantYesterday(TraceWriter log, string requisitionId, string bearerToken, string sessionIdToken)
        {
            //Get URL to execute
            var requisitionUrl = TranscriptAPI.GetRequisitionDetailsURL(requisitionId);

            //Execute URL and return results
            string trainingMetrics = string.Empty;

            ApiAiResponse response = new ApiAiResponse();
            string responseText;
            try
            {
                JObject postingobject = JObject.Parse(await RequestHelper.ExecuteUrl(requisitionUrl, bearerToken, sessionIdToken));
                JToken result = postingobject["data"].FirstOrDefault();
                string numberOfApplicants = result["applicantCount"].ToString();
                string newSubmissionCount = result["newSubmissionCount"].ToString();

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

        public static async Task<ApiAiResponse> GetTopApplicants(TraceWriter log, int requisitionId, string bearerToken, string sessionIdToken)
        {
            //Get URL to execute
            var requisitionUrl = TranscriptAPI.GetCandidateInRequisitionURL(requisitionId, CandidateType.Candidate, 1);

            //Execute URL and return results
            string trainingMetrics = string.Empty;

            ApiAiResponse response = new ApiAiResponse();
            string responseText;
            try
            {
                JObject postingobject = JObject.Parse(await RequestHelper.ExecuteUrl(requisitionUrl, bearerToken, sessionIdToken));
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
