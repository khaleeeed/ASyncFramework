using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Entities
{
    public class PushFailuerEntity
    {
        public long Id { get; set; }

        public long? NotificationId { get; set; }
        public string Queues { get; set; }
        public int Retry { get; set; }
        public bool IsCallBackMessage { get; set; }
        public bool IsFailureMessage { get; set; }
        public DateTime? CreationDate { get; set; }

        public bool? IsActive { get; set; }

        public virtual NotificationEntity Notification { get; set; }
    }
}
