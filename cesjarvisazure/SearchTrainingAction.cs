using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cesjarvisazure
{
    public static class SearchTrainingAction
    {
        public const int PAGE_SIZE = 3;
        public static async Task<ApiAiResponse> SearchTrainings(TraceWriter log, int userId, string bearerToken, string sessionIdToken, int pageNumber)
        {
            //Get URL to execute
            var trainingDetailssUrl = TranscriptAPI.GetTranscriptUrl(userId, pageNumber);
            //Execute URL and return results
            string trainingDetails = string.Empty;

            ApiAiResponse response = new ApiAiResponse();
            string responseText = string.Empty;
            try
            {
                string feedback = await RequestHelper.ExecuteUrl(trainingDetailssUrl, bearerToken, sessionIdToken);
                JObject trainingMetricsJobject = JObject.Parse(feedback);
                JToken trainingMetricsJobjectData = trainingMetricsJobject["data"];

                int recordCount = trainingMetricsJobjectData.Count();
                int numberOfPages = recordCount / PAGE_SIZE;
                int remainingRecords = recordCount % PAGE_SIZE;

                int skipRecords = (pageNumber - 1) * PAGE_SIZE;

                var pagedResults = trainingMetricsJobjectData.Skip(skipRecords).Take(PAGE_SIZE);

                if(pagedResults.Any())
                {
                    responseText = string.Join(". ", pagedResults.Select(x => x["title"].ToString()));                    
                }
                
            }
            catch (Exception ex)
            {
                responseText = "Something went wrong. Please try again.";
            }

            response.displayText = responseText;
            response.speech = responseText;

            dynamic parametersInput = new System.Dynamic.ExpandoObject();
            parametersInput.page_num = pageNumber;

            response.contextOut = new List<Context>() {
                new Context{
                    name = "search-training",
                    parameters = parametersInput,
                    lifespan = 5
                },
            };

            return response;

        }
    }
}
