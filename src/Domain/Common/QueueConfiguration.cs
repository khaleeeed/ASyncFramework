using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ASyncFramework.Domain.Common
{
    public class QueueConfiguration
    {
        public int QueueRetry { get; set; }
        public int Dealy { get; set; }
        public string QueueName { get; set; }
        public string ExhangeName { get; set; }
    }
}
