using ASyncFramework.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Model.Request
{
    public class Request:BaseRequest
    {
        public MethodVerb MethodVerb { get; set; }
        public object ContentBody { get; set; }
        public Dictionary<string, string> Headers { get; set; }

    }
}
