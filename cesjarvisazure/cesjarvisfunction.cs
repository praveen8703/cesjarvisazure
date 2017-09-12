using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace cesjarvisazure
{
	public static class cesjarvisfunction
	{
		[FunctionName("cesjarvisfunction")]
		public static async Task<ApiAiResponse> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
		{
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
					return await UserFunctions.SearchName(log, searchParameter, bearerToken, sessionIdToken);

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

				case "search.applicant":
					return await SearchApplicant(request, log, bearerToken, sessionIdToken);

				case "searchapplicant.searchapplicant-repeat":
					return await SearchApplicantRepeat(request, log, bearerToken, sessionIdToken);

				case "searchapplicant.searchapplicant-next":
					return await SearchApplicantNext(request, log, bearerToken, sessionIdToken);

				case "searchapplicant.searchapplicant-previous":
					return await SearchApplicantPrevious(request, log, bearerToken, sessionIdToken);

				case "searchapplicant.searchapplicant-selectnumber":
					return await SearchApplicantSelect(request, log, bearerToken, sessionIdToken);

				case "create.job.posting":
					log.Info($"Inside {request.result.action.ToLowerInvariant()} action");
					return await ATSFunctions.CreatePosting(log, 609, 44, bearerToken, sessionIdToken);

				case "remove.job.posting":
					log.Info($"Inside {request.result.action.ToLowerInvariant()} action");
					return await ATSFunctions.RemovePosting(log, 609, bearerToken, sessionIdToken);

				default:
					return await DefaultResponse.GetDefaultResponse();
			}
		}

		#region Search Applicants

		private static async Task<ApiAiResponse> SearchApplicant(ApiAiRequest request, TraceWriter log, string bearerToken, string sessionIdToken)
		{
			log.Info($"Inside {request.result.action.ToLowerInvariant()} action");
			var context = request.result.contexts.FirstOrDefault(x => x.name == "search-applicant");
			int reqId = Convert.ToInt32(context.parameters.req_id.Value);
			int pageNum = 1;
			return await ATSFunctions.SearchApplicants(log, reqId, bearerToken, sessionIdToken, pageNum, PagingAction.Init);
		}
		private static async Task<ApiAiResponse> SearchApplicantRepeat(ApiAiRequest request, TraceWriter log, string bearerToken, string sessionIdToken)
		{
			log.Info($"Inside {request.result.action.ToLowerInvariant()} action");
			var context = request.result.contexts.FirstOrDefault(x => x.name == "search-applicant");
			int reqId = Convert.ToInt32(context.parameters.req_id.Value);
			int pageNum = Convert.ToInt32(context.parameters.page_num.Value);
			return await ATSFunctions.SearchApplicants(log, reqId, bearerToken, sessionIdToken, pageNum, PagingAction.Repeat);
		}
		private static async Task<ApiAiResponse> SearchApplicantNext(ApiAiRequest request, TraceWriter log, string bearerToken, string sessionIdToken)
		{
			log.Info($"Inside {request.result.action.ToLowerInvariant()} action");
			var context = request.result.contexts.FirstOrDefault(x => x.name == "search-applicant");
			int reqId = Convert.ToInt32(context.parameters.req_id.Value);
			int total = Convert.ToInt32(context.parameters.total.Value);
			int newPageNum = Convert.ToInt32(context.parameters.page_num.Value) + 1;
			int pageNum = ((newPageNum * 3) > total) ? 1 : newPageNum;

			return await ATSFunctions.SearchApplicants(log, reqId, bearerToken, sessionIdToken, pageNum, PagingAction.Next);
		}
		private static async Task<ApiAiResponse> SearchApplicantPrevious(ApiAiRequest request, TraceWriter log, string bearerToken, string sessionIdToken)
		{
			log.Info($"Inside {request.result.action.ToLowerInvariant()} action");
			var context = request.result.contexts.FirstOrDefault(x => x.name == "search-applicant");
			int reqId = Convert.ToInt32(context.parameters.req_id.Value);
			int newPageNum = Convert.ToInt32(context.parameters.page_num.Value) - 1;
			int pageNum = (newPageNum == 0) ? 1 : newPageNum;
			return await ATSFunctions.SearchApplicants(log, reqId, bearerToken, sessionIdToken, pageNum, PagingAction.Previous);
		}
		private static async Task<ApiAiResponse> SearchApplicantSelect(ApiAiRequest request, TraceWriter log, string bearerToken, string sessionIdToken)
		{
			log.Info($"Inside {request.result.action.ToLowerInvariant()} action");
			var context = request.result.contexts.FirstOrDefault(x => x.name == "search-applicant");
			int reqId = Convert.ToInt32(context.parameters.req_id.Value);
			int pageNum = Convert.ToInt32(context.parameters.page_num.Value);
			int selectedNum = Convert.ToInt32(context.parameters.number.Value);
			return await ATSFunctions.SelectApplicant(log, reqId, bearerToken, sessionIdToken, pageNum, selectedNum);
		}

		#endregion
	}
}