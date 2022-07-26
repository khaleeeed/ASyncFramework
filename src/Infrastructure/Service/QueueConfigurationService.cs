using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Infrastructure.Service
{
    public class QueueConfigurationService : IQueueConfigurationService
    {
        private readonly IQueueConfigurationRepository _QueueConfigurationRepository;
        private Dictionary<string, QueueConfigurations> _QueueConfiguration;
        public QueueConfigurationService(IQueueConfigurationRepository queueConfigurationRepository)
        {
            _QueueConfigurationRepository = queueConfigurationRepository;

            var queueConfigurations =  _QueueConfigurationRepository.GetAll().Result;

            Set_QueueConfiguration(queueConfigurations);
        }


        public Dictionary<string, QueueConfigurations> QueueConfiguration => _QueueConfiguration;

        public void UpdateQueueConfiguration()
        {
            var queueConfigurations = _QueueConfigurationRepository.GetAll().Result;
            Set_QueueConfiguration(queueConfigurations);
        }

        private void Set_QueueConfiguration(IEnumerable<QueueConfigurations> queueConfigurations)
        {
            Dictionary<string, QueueConfigurations> queueConfigurationDictionary = new Dictionary<string, QueueConfigurations>();

            foreach (var queue in queueConfigurations)
            {
                queueConfigurationDictionary.Add(queue.ID.ToString(), queue);
            }

            _QueueConfiguration = queueConfigurationDictionary;
        }
    }
}
