using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Model.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Model
{
    public class Message
    {
        public string ReferenceNumber { get; set; }
        public string Queues { get; set; }
        public int Retry { get; set; }
        public bool IsCallBackMessage { get; set; }
        public string HttpStatusCode { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public Request.Request TargetOAuthRequest { get; set; }
        public PushRequest TargetRequest { get; set; }

        public CallBackRequest CallBackRequest { get; set; }
        public Request.Request CallBackOAuthRequest { get; set; }
    }
}
