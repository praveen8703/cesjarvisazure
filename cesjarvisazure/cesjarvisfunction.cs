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
            #region SESSIONDETAILS

            #region LA4PIL
            // Bearer Token
            IEnumerable<string> bearerTokens = new List<string>();
			req.Headers.TryGetValues(HeaderNames.BEARERTOKENHEADER, out bearerTokens);
			if (!bearerTokens.Any())
			{
				log.Warning("Cannot find bearer token from header");
			}
			string bearerToken = bearerTokens.FirstOrDefault();

			// Session Token
			IEnumerable<string> sessionIdTokens = new List<string>();
			req.Headers.TryGetValues(HeaderNames.ASPSESSIONIDHEADER, out sessionIdTokens);
			if (!sessionIdTokens.Any())
			{
				log.Warning("Cannot find session id token from header");
			}
			string sessionIdToken = sessionIdTokens.FirstOrDefault();

			// UserId
			IEnumerable<string> userIdTokens = new List<string>();
			req.Headers.TryGetValues(HeaderNames.USERIDHEADER, out userIdTokens);
			if (!userIdTokens.Any())
			{
				log.Warning("Cannot find userid token from header");
			}
			string userIdToken = userIdTokens.FirstOrDefault();
			int userId = int.Parse(userIdToken);
            #endregion

            #region DOGFOOD
            // Bearer Token
            IEnumerable<string> bearerTokensDogfood = new List<string>();
            req.Headers.TryGetValues(HeaderNames.BEARERTOKENHEADERDOGFOOD, out bearerTokensDogfood);
            if (!bearerTokensDogfood.Any())
            {
                log.Warning("Cannot find bearer token from header");
            }
            string bearerTokenDogfood = bearerTokensDogfood.FirstOrDefault();

            // Session Token
            IEnumerable<string> sessionIdTokensDogfood = new List<string>();
            req.Headers.TryGetValues(HeaderNames.ASPSESSIONIDHEADERDOGFOOD, out sessionIdTokensDogfood);
            if (!sessionIdTokensDogfood.Any())
            {
                log.Warning("Cannot find session id token from header");
            }
            string sessionIdTokenDogfood = sessionIdTokensDogfood.FirstOrDefault();

            // UserId
            IEnumerable<string> userIdTokensDogfood = new List<string>();
            req.Headers.TryGetValues(HeaderNames.USERIDHEADERDOGFOOD, out userIdTokensDogfood);
            if (!userIdTokensDogfood.Any())
            {
                log.Warning("Cannot find userid token from header");
            }
            string userIdTokenDogfood = userIdTokensDogfood.FirstOrDefault();
            int userIdDogfood = int.Parse(userIdTokenDogfood);
            #endregion


            #endregion

            // Get request body
            ApiAiRequest request = await req.Content.ReadAsAsync<ApiAiRequest>();

			switch (request.result.action.ToLowerInvariant())
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
					if (pageNumberPrev <= 0)
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
					return await UserFunctions.SearchName(log, searchParameter, bearerTokenDogfood, sessionIdTokenDogfood);

				case "get.top.applicants":
					log.Info($"Inside {request.result.action.ToLowerInvariant()} action");
					var searchContextApplicants = request.result.contexts.FirstOrDefault(x => x.name == "get-top-applicants");
					int jobReqId = Convert.ToInt32(searchContextApplicants.parameters.req_id.Value);
					return await ATSFunctions.GetTopApplicants(log, jobReqId, bearerToken, sessionIdToken);

				case "get.applicant.count":
					log.Info($"Inside {request.result.action.ToLowerInvariant()} action");
					var searchContextApplicantCount = request.result.contexts.FirstOrDefault(x => x.name == "get-applicant-count");
					int jobReqId2 = Convert.ToInt32(searchContextApplicantCount.parameters.req_id.Value);
					return await ATSFunctions.GetApplicantCount(log, jobReqId2, bearerToken, sessionIdToken);

                case "create.job.posting":
                    log.Info($"Inside {request.result.action.ToLowerInvariant()} action");
                    return await ATSFunctions.CreatePosting(log, 609, 44, bearerToken, sessionIdToken);

                case "remove.job.posting":
                    log.Info($"Inside {request.result.action.ToLowerInvariant()} action");
                    return await ATSFunctions.RemovePosting(log, 609,bearerToken, sessionIdToken);

                default:
					return await DefaultResponse.GetDefaultResponse();
			}

		}
	}
}