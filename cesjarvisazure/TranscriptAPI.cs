using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cesjarvisazure
{
    public static class TranscriptAPI
    {
        public static string baseUrl = "https://la4prdsl1.csod.com";
       
        public static string GetTrainingMetricsURL(int userId)
        {
            string url = string.Format("/services/api/lms/user/{0}/transcript/metrics", userId);
            return baseUrl + url;
        }

        public static string GetTranscriptUrl(int userId, bool isCompleted = false, bool isArchived = false, bool isRemoved = false, bool isStandAlone = true, int pageSize = 20, int pageNumber = 1)
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

        public static string GetRequisitionDetailsURL(string requisitionId)
        {
            string url = string.Format("/services/api/ATS/JobRequisition?ReqId={0}", requisitionId);
            return baseUrl + url;
        }

        public static string GetCandidateInRequisitionURL(int requisitionId, CandidateType candidateType = CandidateType.Candidate, int pageNumber = 1)
        {
            string url = string.Format("/services/api/ATS/JobRequisition/{0}/Applicants/?type={1}&page={2}&pageSize={3}", candidateType.ToString(), requisitionId, pageNumber, 3);
            return baseUrl + url;
        }

        public static string SearchName(string searchTerms)
        {
            string url = string.Format("/services/api/Search/?name={0}", searchTerms);
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
