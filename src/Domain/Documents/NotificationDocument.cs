using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ASyncFramework.Domain.Documents
{
    public class NotificationDocument
    {
        [DataMember(Name = "fields")]
        public List<NotificationFields> Fields { get; set; }

        [DataMember(Name = "SystemCode")]
        public int SystemCode { get; set; }
        [DataMember(Name = "SystemName")]
        public string SystemName { get; set; }
        [DataMember(Name = "SystemArName")]
        public string SystemArName { get; set; }
        
    }
    public class NotificationFields
    {
        [DataMember(Name= "Name")]
        public string Name { get; set; }
        [DataMember(Name = "ArName")]
        public string ArName { get; set; }
        [DataMember(Name = "Description")]
        public string Description { get; set; }
        [DataMember(Name = "SamplePayload")]
        public object SamplePayload { get; set; }
        [DataMember(Name = "Type")]
        public string Type { get; set; }
        [DataMember(Name = "ReferenceNumber")]
        public string ReferenceNumber { get; set; }
        [DataMember(Name = "CreationDate")]
        public DateTime CreationDate { get; set; }
        [DataMember(Name = "ContentType")]
        public string ContentType { get; set; }
    }
}
