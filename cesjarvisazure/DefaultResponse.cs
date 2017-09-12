using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cesjarvisazure
{
    public static class DefaultResponse
    {
        public static async Task<ApiAiResponse> GetDefaultResponse()
        {
            ApiAiResponse response = new ApiAiResponse();
            string errorResponse = "Did not find action - Something went wrong. Please try again.";
            response.displayText = errorResponse;
            response.speech = errorResponse;
            return response;
        }

    }
}
