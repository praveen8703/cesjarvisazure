using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;

namespace cesjarvisazure
{
    public static class GetNumberOfTrainingsAssigned
    {
        public static async Task<ApiAiResponse> GetTrainingMetrics(TraceWriter log, int userId, string bearerToken, string sessionIdToken)
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
    }
}