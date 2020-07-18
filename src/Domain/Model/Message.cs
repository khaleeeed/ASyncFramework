using ASyncFramework.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Model
{
    public class Message
    {
        public string ReferenceNumber { get; set; }
        public Dictionary<string,string> Headers { get;  set; }
        public string OAuthHttpCode { get; set; }
        public string ContentBody { get; set; }
        public string TargetUrl { get; set; }
        public string CallBackUrl { get; set; }
        public string Queues { get; set; }
        public TargetVerb TargetVerb { get; set; }
        public int Retry { get; set; }
        public bool IsCallBackMessage { get; set; }
        public string OAuthHttpCodeCallBack { get; set; }
        public string HttpStatusCode { get; set; }

    }
}
