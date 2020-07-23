using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Model.Request
{
    public class CallBackRequest:BaseRequest
    {
        public Dictionary<string, string> Headers { get; set; }

    }
}