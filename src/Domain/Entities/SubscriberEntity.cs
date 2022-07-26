using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ASyncFramework.Domain.Entities
{
    public class SubscriberEntity
    {
        public long ID { get; set; }
        public string Url { get; set; }
        public bool IsRunning { get; set; }
        public DateTime TimeOfTakeConfiguration { get; set; }
        [Timestamp]
        public byte[] TimeStampCheck { get; set; }
    }
}
