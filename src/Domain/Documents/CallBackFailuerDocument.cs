using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ASyncFramework.Domain.Documents
{

    [DataContract(Name = "asyncframework-CallBack-*")]
    public class CallBackFailuerDocument
    {        
        [DataMember(Name = "fields")]
        public CallBackFields Fields { get; set; }
    }
    public class CallBackFields
    {
        [DataMember(Name = "ReferenceNumber")]
        public string ReferenceNumber { get; set; }
        [DataMember(Name = "StatusCode")]
        public int? StatusCode { get; set; } 
        [DataMember(Name = "CallBackUrl")]
        public string CallBackUrl { get; set; }
        [DataMember(Name = "ResponseContent")]
        public object ResponseContent { get; set; }
        [DataMember(Name = "CreationDate")]
        public DateTime CreationDate { get; set; }
        [DataMember(Name = "Retry")]
        public int Retry { get; set; }
        [DataMember(Name = "IsSendSuccessfully")]
        public bool IsSendSuccessfully { get; set; }
        [DataMember(Name = "SystemCode")]
        public string SystemCode { get; set; }
        [DataMember(Name = "Message")]
        public string Message { get; set; }
        [DataMember(Name = "IsProcessing")]
        public bool IsProcessing { get; set; }
    }

}
