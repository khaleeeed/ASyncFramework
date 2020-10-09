using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using System.Text;

namespace ASyncFramework.Domain.Documents
{
    [DataContract(Name = "asyncframework-infrastructure-*")]
    public class AsyncframeworkInfrastructureDocument
    {           
        [DataMember(Name ="level")]
        public string Level { get; set; }
        [DataMember(Name = "fields")]
        public Fields Fields { get; set; }
    }
    public class Fields
    {
        [DataMember(Name = "Status")]
        public string Status { get; set; }

        [DataMember(Name = "ReferenceNumber")]
        public string ReferenceNumber { get; set; }

        [DataMember(Name = "StatusCode")]
        public int? StatusCode { get; set; }

        [DataMember(Name = "TargetUrl")]
        public string TargetUrl { get; set; }
        [DataMember(Name = "CallBackUrl")]
        public string CallBackUrl { get; set; }
        [DataMember(Name ="Url")]
        public string Url { get; set; }

        [DataMember(Name = "Retry")]
        public int? Retry { get; set; }

        [DataMember(Name = "Queues")]
        public string Queues { get; set; }

        [DataMember(Name = "ResponseContent")]
        public string ResponseContent { get; set; }
        [DataMember(Name = "CreationDate")]
        public DateTime CreationDate { get; set; }
        [DataMember(Name = "Hash")]
        public string Hash { get; set; }
    }
}
