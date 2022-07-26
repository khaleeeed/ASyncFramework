using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ASyncFramework.Domain.Common
{
    public class QueueConfigurations
    {
        public int ID { get; set; }
        public int QueueRetry { get; set; }
        public int Dealy { get; set; }
        public string QueueName { get; set; }
        public string ExhangeName { get; set; }
        public bool IsAutoMapping { get; set; }
        public int NumberOfInstance { get; set; }
        public string ExhangeType { get; set; }
        public byte[] TimeStampCheck { get; set; }
    }
}
