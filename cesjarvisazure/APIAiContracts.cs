using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cesjarvisazure
{
    public class Context
    {
        public string name { get; set; }
        public dynamic parameters;
        public int lifespan { get; set; }
    }

    public class Metadata
    {
        public string intentId { get; set; }
        public string webhookUsed { get; set; }
        public string webhookForSlotFillingUsed { get; set; }
        public string intentName { get; set; }
    }

    public class Message
    {
        public int type { get; set; }
        public string speech { get; set; }
    }

    public class Fulfillment
    {
        public string speech { get; set; }
        public List<Message> messages { get; set; }
    }

    public class Result
    {
        public string speech { get; set; }
        public string source { get; set; }
        public string resolvedQuery { get; set; }
        public string action { get; set; }
        public bool actionIncomplete { get; set; }
        public dynamic parameters { get; set; }
        public List<Context> contexts { get; set; }
        public Metadata metadata { get; set; }
        public Fulfillment fulfillment { get; set; }
        public decimal score { get; set; }
    }

    public class Status
    {
        public int code { get; set; }
        public string errorType { get; set; }
    }

    public class ApiAiResponse
    {
        public string speech;
        public string displayText;
        public string source { get; set; }
        public object data { get; set; }
        public List<Context> contextOut { get; set; }

        public ApiAiResponse(List<Context> request_ctx)
        {
            contextOut = new List<Context>(request_ctx);
            source = "<your-source>";
        }
        public ApiAiResponse() { }

    }

    public class ApiAiRequest
    {
        public OriginalRequest originalRequest { get; set; }
        public string id { get; set; }
        public string timestamp { get; set; }
        public Result result { get; set; }
        public Status status { get; set; }
        public string sessionId { get; set; }
        public bool resetContexts { get; set; } = false;
    }

    public class OriginalRequest
    {
        public object data { get; set; }
        public string source { get; set; }
    }
}
