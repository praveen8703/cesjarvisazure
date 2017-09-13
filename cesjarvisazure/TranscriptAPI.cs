using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cesjarvisazure
{
	public static class TranscriptAPI
	{
		public static string baseUrlDogfood = "https://cornerstone.csod.com/";
		public static string baseUrl = "https://la4prdsl1.csod.com/";

		public static string GetTrainingMetricsURL(int userId)
		{
			string url = string.Format("/services/api/lms/user/{0}/transcript/metrics", userId);
			return baseUrl + url;
		}

		public static string GetTranscriptUrl(int userId, int pageNumber = 1, bool isCompleted = false, bool isArchived = false, bool isRemoved = false, bool isStandAlone = true, int pageSize = 30)
		{
			string url = string.Format("/services/api/lms/user/{0}/transcript?isCompleted={1}&isArchived={2}&isRemoved={3}&isStandAlone={4}&sortCriteria=StatusChangeDate&pageSize={5}&pageNum={6}"
					, userId
					, isCompleted ? "true" : "false"
					, isArchived ? "true" : "false"
					, isRemoved ? "true" : "false"
					, isStandAlone ? "true" : "false"
					, pageSize
					, pageNumber);
			return baseUrl + url;
		}

		public static string GetJobRequisitionURL(int requisitionId)
		{
			//string url = string.Format("/services/api/ATS/JobRequisition?ReqId={0}", requisitionId);
			string url = string.Format("/services/api/ATS/JobRequisition/{0}", requisitionId);
			return baseUrl + url;
		}

		public static string GetApplicantsURL(int requisitionId)
		{
			string url = string.Format("/services/api/ATS/JobRequisition/{0}/Applicants/?type=Candidate&page=1&pageSize=3", requisitionId);
			return baseUrl + url;
		}

		public static string SearchName(string searchTerms)
		{
			string url = string.Format("/services/api/Search/?name={0}", searchTerms);
			return baseUrlDogfood + url;
		}

		public static string PostingURL(int requisitionId)
		{
			string url = string.Format("/services/x/career-site/v1/requisition/{0}", requisitionId);
			return baseUrl + url;
		}
	}

	public enum CandidateType
	{
		Candidate, NewSubmission
	}

	public class TranscriptAPIMetrics
	{
		public int assignedCount { get; set; }
		public int hasDueDateCount { get; set; }
		public int pastDueCount { get; set; }
		public int noDueDateCount { get; set; }
		public int dueSoonCount { get; set; }
		public int completedCount { get; set; }
		public int totalHours { get; set; }
	}

}