using ASyncFramework.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Model
{
    public class PushRequest
    {
        public string OAuthHttpCode { get; set; }
        public object ContentBody { get; set; }
        public string TargetUrl { get; set; }
        public string CallBackUrl { get; set; }
        public string Queue { get; set; }
        public TargetVerb TargetVerb { get; set; }
        public string CallBackOAuthHttpCode { get; set; }
    }
}