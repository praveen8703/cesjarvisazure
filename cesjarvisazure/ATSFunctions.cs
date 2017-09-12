using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

				var responseJSON = await RequestHelper.ExecuteUrl(postingUrl, bearerToken, sessionIdToken, jsonStuff, HttpMethod.Put.Method);
				JObject postingobject = JObject.Parse(responseJSON);
				JToken result = postingobject["data"].FirstOrDefault();

				responseText = $"Posting has been created successfully for your career site.";
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
				var responseJSON = await RequestHelper.ExecuteUrl(postingUrl, bearerToken, sessionIdToken, "[]", HttpMethod.Put.Method);
				JObject postingobject = JObject.Parse(responseJSON);
				JToken result = postingobject["data"].FirstOrDefault();

				responseText = $"Posting has been removed successfully from your career site.";
			}
			catch (Exception ex)
			{
				responseText = "Something went wrong. Please try again.";
			}

			response.displayText = responseText;
			response.speech = responseText;
			return response;
		}

		public static async Task<ApiAiResponse> GetApplicantCount(TraceWriter log, int requisitionId, string bearerToken, string sessionIdToken)
		{
			//Get URL to execute
			var requisitionUrl = TranscriptAPI.GetJobRequisitionURL(requisitionId);

			//Execute URL and return results
			string trainingMetrics = string.Empty;

			ApiAiResponse response = new ApiAiResponse();
			string responseText;
			try
			{
				string responseString = await RequestHelper.ExecuteUrl(requisitionUrl, bearerToken, sessionIdToken);
				JObject postingobject = JObject.Parse(responseString);
				JToken fields = postingobject["data"].FirstOrDefault()["items"].FirstOrDefault()["fields"];
				int applicantCount = Convert.ToInt32(fields["applicantCount"]);
				int newSubmissionCount = Convert.ToInt32(fields["newSubmissionCount"]);

				string applicationResponse = (applicantCount == 1) ? $"You have {applicantCount} application" : $"You have {applicantCount} applications";
				string newSubmissionResponse = (newSubmissionCount == 1) ? $"and there is {newSubmissionCount} new submission." : $"and there are {newSubmissionCount} new submissions.";
				responseText = $"{applicationResponse} {newSubmissionResponse}";
			}
			catch (Exception ex)
			{
				responseText = "Sorry, I can't get the applicant count right now. Please try again later.";
			}

			response.displayText = responseText;
			response.speech = responseText;
			return response;
		}

		public static async Task<ApiAiResponse> GetTopApplicants(TraceWriter log, int requisitionId, string bearerToken, string sessionIdToken)
		{
			//Get URL to execute
			var requisitionUrl = TranscriptAPI.GetApplicantsURL(requisitionId);

			//Execute URL and return results
			string trainingMetrics = string.Empty;

			ApiAiResponse response = new ApiAiResponse();
			string responseText;
			try
			{
				string responseString = await RequestHelper.ExecuteUrl(requisitionUrl, bearerToken, sessionIdToken);
				JObject postingobject = JObject.Parse(responseString);
				string totalRecords = postingobject["totalRecords"].ToString();
				JToken result = postingobject["data"].FirstOrDefault();
				string results = string.Join(", ", result["items"].Take(3).Select(x => x["fields"]["name"]));
				responseText = $"There are {totalRecords} results. The top 3 applicants are: {results}.";
			}
			catch (Exception ex)
			{
				responseText = "Sorry, I can't get the top applicants right now. Please try again later.";
			}

			response.displayText = responseText;
			response.speech = responseText;
			return response;
		}

		public static async Task<ApiAiResponse> SearchApplicants(TraceWriter log, int requisitionId, string bearerToken, string sessionIdToken, int pageNum, PagingAction paging)
		{
			//Get URL to execute
			var requisitionUrl = TranscriptAPI.GetApplicantsURL(requisitionId, pageNum);

			//Execute URL and return results
			string trainingMetrics = string.Empty;

			ApiAiResponse response = new ApiAiResponse();
			string responseText = string.Empty;
			int totalRecords = 0;
			try
			{
				string responseString = await RequestHelper.ExecuteUrl(requisitionUrl, bearerToken, sessionIdToken);
				JObject postingobject = JObject.Parse(responseString);
				totalRecords = Convert.ToInt32(postingobject["totalRecords"]);
				JToken result = postingobject["data"].FirstOrDefault();
				string appNames = string.Join(", ", result["items"].Take(3).Select(x => x["fields"]["name"]));

				if (paging == PagingAction.Init)
					responseText = $"There are {totalRecords} results. The top 3 applicants are: {appNames}.";
				else if (paging == PagingAction.Next)
					responseText = $"The next applicants are: {appNames}.";
				else if (paging == PagingAction.Previous)
					responseText = $"The previous applicants are: {appNames}.";
				else if (paging == PagingAction.Repeat)
					responseText = $"Here are the results again: {appNames}.";
			}
			catch (Exception ex)
			{
				responseText = "Sorry, I can't search for applicants right now. Please try again later.";
			}

			response.displayText = responseText;
			response.speech = responseText;

			dynamic contextOutParam = new System.Dynamic.ExpandoObject();
			contextOutParam.page_num = pageNum;
			contextOutParam.total = totalRecords;
			response.contextOut = new List<Context>() {
				new Context{
					name = "search-applicant",
					parameters = contextOutParam,
					lifespan = 5
				},
			};

			return response;
		}

		public static async Task<ApiAiResponse> SelectApplicant(TraceWriter log, int requisitionId, string bearerToken, string sessionIdToken, int pageNum, int selectedNum)
		{
			//Get URL to execute
			var requisitionUrl = TranscriptAPI.GetApplicantsURL(requisitionId, pageNum);

			//Execute URL and return results
			string trainingMetrics = string.Empty;

			ApiAiResponse response = new ApiAiResponse();
			string responseText = string.Empty;
			int totalRecords = 0;
			try
			{
				string responseString = await RequestHelper.ExecuteUrl(requisitionUrl, bearerToken, sessionIdToken);
				JObject postingobject = JObject.Parse(responseString);
				totalRecords = Convert.ToInt32(postingobject["totalRecords"]);
				JToken result = postingobject["data"].FirstOrDefault();
				string appName = result["items"][selectedNum - 1]["fields"]["name"].ToString();
				string email = result["items"][selectedNum - 1]["fields"]["email"].ToString();
				string phone = result["items"][selectedNum - 1]["fields"]["phone"].ToString();
				responseText = $"And the winner by popular vote is {appName}. You can email your complaints to {email}. Or call him at {phone}." ;
			}
			catch (Exception ex)
			{
				responseText = "Sorry, I can't select that applicant right now. Please try again later.";
			}

			response.displayText = responseText;
			response.speech = responseText;

			return response;
		}
	}
}