using System.Threading.Tasks;

namespace cesjarvisazure
{
	public static class DefaultResponse
    {
        public static async Task<ApiAiResponse> GetDefaultResponse()
        {
            ApiAiResponse response = new ApiAiResponse();
            string errorResponse = "I'm not sure how to do that yet.";
            response.displayText = errorResponse;
            response.speech = errorResponse;
            return response;
        }

    }
}
