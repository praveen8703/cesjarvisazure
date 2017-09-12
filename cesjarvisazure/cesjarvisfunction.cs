using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using System;

namespace cesjarvisazure
{
    public static class cesjarvisfunction
    {
        [FunctionName("cesjarvisfunction")]
        public static async Task<ApiAiResponse> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            IEnumerable<string> bearerTokens = new List<string>();
            req.Headers.TryGetValues(HeaderNames.BEARERTOKENHEADER, out bearerTokens);
            if (!bearerTokens.Any())
            {
                log.Warning("Cannot find bearer token from header");
            }
            string bearerToken = bearerTokens.FirstOrDefault();

            IEnumerable<string> sessionIdTokens = new List<string>();
            req.Headers.TryGetValues(HeaderNames.ASPSESSIONIDHEADER, out sessionIdTokens);
            if (!sessionIdTokens.Any())
            {
                log.Warning("Cannot find session id token from header");
            }
            string sessionIdToken = sessionIdTokens.FirstOrDefault();

            IEnumerable<string> userIdTokens = new List<string>();
            req.Headers.TryGetValues(HeaderNames.USERIDHEADER, out userIdTokens);
            if (!userIdTokens.Any())
            {
                log.Warning("Cannot find userid token from header");
            }
            string userIdToken = userIdTokens.FirstOrDefault();
            int userId = int.Parse(userIdToken);


            // Get request body
            ApiAiRequest request = await req.Content.ReadAsAsync<ApiAiRequest>();

            switch(request.result.action.ToLowerInvariant())
            {
                case "training.summary":
                    log.Info($"Inside {request.result.action.ToLowerInvariant()} action");
                    return await GetNumberOfTrainingsAssigned.GetTrainingMetrics(log, userId, bearerToken, sessionIdToken);
                case "search.training":
                case "search.training.repeat":
                    log.Info($"Inside {request.result.action.ToLowerInvariant()} action");
                    var searchContext = request.result.contexts.FirstOrDefault(x => x.name == "search-training");
                    int pageNumber = Convert.ToInt32(searchContext.parameters.page_num.Value);
                    return await SearchTrainingAction.SearchTrainings(log, userId, bearerToken, sessionIdToken, pageNumber);
                case "searchtraining.searchtraining-next":
                    log.Info($"Inside {request.result.action.ToLowerInvariant()} action");
                    var searchContextNext = request.result.contexts.FirstOrDefault(x => x.name == "search-training");
                    int pageNumberNext = Convert.ToInt32(searchContextNext.parameters.page_num.Value) + 1;
                    return await SearchTrainingAction.SearchTrainings(log, userId, bearerToken, sessionIdToken, pageNumberNext);
                case "searchtraining.searchtraining-previous":
                    log.Info($"Inside {request.result.action.ToLowerInvariant()} action");
                    var searchContextPrev = request.result.contexts.FirstOrDefault(x => x.name == "search-training");
                    int pageNumberPrev = Convert.ToInt32(searchContextPrev.parameters.page_num.Value) - 1;
                    if(pageNumberPrev<=0)
                    {
                        pageNumberPrev = 1;
                    }
                    return await SearchTrainingAction.SearchTrainings(log, userId, bearerToken, sessionIdToken, pageNumberPrev);

                case "search.users":
                    log.Info($"Inside {request.result.action.ToLowerInvariant()} action");
                    var searchUserContext = request.result.contexts.FirstOrDefault(x => x.name == "search-users");
                    string searchParameter = string.Empty;
                    if (searchUserContext.parameters.search_key != null)
                    {
                        searchParameter = Convert.ToString(searchUserContext.parameters.search_key.Value);
                    }
                    return await UserFunctions.SearchName(log, searchParameter, bearerToken, sessionIdToken);
                default:
                    return await DefaultResponse.GetDefaultResponse();
            }

        }
    }
}
