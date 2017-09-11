using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cesjarvisazure
{
    public static class TranscriptAPI
    {
        public static string GetTrainingMetricsURL(int userId)
        {
            string url = string.Format("https://cornerstone.csod.com/services/api/lms/user/{0}/transcript/metrics", userId);
            return url;
        }

        public static string GetTranscriptUrl(int userId, bool isCompleted = false, bool isArchived = false, bool isRemoved = false, bool isStandAlone = true, int pageSize = 20, int pageNumber = 1)
        {
            string url = string.Format("https://cornerstone.csod.com/services/api/lms/user/{0}/transcript?isCompleted={1}&isArchived={2}&isRemoved={3}&isStandAlone={4}&sortCriteria=StatusChangeDate&pageSize={5}&pageNum={6}"
                , userId
                , isCompleted ? "true" : "false"
                , isArchived ? "true" : "false"
                , isRemoved ? "true" : "false"
                , isStandAlone ? "true" : "false"
                , pageSize
                , pageNumber);
            return url;
        }
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
