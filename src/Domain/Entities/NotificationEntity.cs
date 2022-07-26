using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Entities
{
    public class NotificationEntity
    {
        public long Id { get; set; }
       
        public string Request { get; set; }

        public string Hash { get; set; }

        public string ExtraInfo { get; set; }

        public string SystemCode { get; set; }

        public string TargetUrl { get; set; }

        public string CallBackUrl { get; set; }

        public DateTime? CreationDate { get; set; }

        public string Header { get; set; }

        public int MessageLifeCycleId { get; set; }

        public int? ServiceCode { get; set; }
    }
}
