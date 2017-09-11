using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace cesjarvisazure
{
    public static class RealGrootFunction
    {
        [FunctionName("RealGrootFunction")]
        public static async Task<ApiAiResponse> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "ces/getresponse")]HttpRequestMessage req, TraceWriter log)
        {

            log.Info("C# HTTP trigger function processed a RealGrootFunction request.");

            // Get request body
            ApiAiRequest data = await req.Content.ReadAsAsync<ApiAiRequest>();

            // Set name to query string or body data

            var response = new ApiAiResponse
            {
                speech = "I AM GROOT",
                displayText = "I AM GROOT"
            };

            return response;
        }
    }
}
