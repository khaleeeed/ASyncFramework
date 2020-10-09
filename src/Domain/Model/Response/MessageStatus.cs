using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Model.Response
{
    public class MessageStatus
    {     
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public string Timestamp { get; set; }
        public string MyProperty { get; set; }
    }
}
