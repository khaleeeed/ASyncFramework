using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Entities
{
    public class CallBackFailuerEntity
    {
        public long Id { get; set; }
        public long NotificationId { get; set; }

        public string CallBackUrl { get; set; }

        public string ContentBody { get; set; }

        public string Method { get; set; }

        public int? Retry { get; set; }

        public bool? IsSendSuccessfully { get; set; }

        public int? StatusCode { get; set; }

        public string SystemCode { get; set; }

        public string Message { get; set; }

        public bool? IsProcessing { get; set; }
        public byte[] TimeStampCheck { get; set; }

        public DateTime? CreationDate { get; set; }

        public virtual NotificationEntity Notification { get; set; }
    }
}
