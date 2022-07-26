using ASyncFramework.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Domain.Interface
{
    public interface IQueueConfigurationService
    {
        public Dictionary<string,QueueConfigurations> QueueConfiguration { get; }
        public void UpdateQueueConfiguration();

    }
}
