using ASyncFramework.Domain.Model.Request;
using System.Collections.Generic;

namespace ASyncFramework.Domain.Model
{
    public class Message
    {
        public string ReferenceNumber { get; set; }
        public string Queues { get; set; }
        public string CallBackQueues { get; set; }
        public int Retry { get; set; }
        public bool IsCallBackMessage { get; set; }
        public bool IsFailureMessage { get; set; }
        public bool HasCustomQueue { get; set; }
        public int? HttpStatusCode { get; set; }
        public string SystemCode { get; set; }
        public int ServiceCode { get; set; }
        public string ExtraInfo { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public PushRequest TargetRequest { get; set; }

        public List<CallBackRequestModel> CallBackRequest { get; set; }
    }
}