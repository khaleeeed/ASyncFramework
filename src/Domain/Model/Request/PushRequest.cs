using ASyncFramework.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace ASyncFramework.Domain.Model.Request
{
    public class PushRequest:BaseRequest
    {
        public MethodVerb MethodVerb { get; set; }
        public string ContentBody { get; set; }
    }
}