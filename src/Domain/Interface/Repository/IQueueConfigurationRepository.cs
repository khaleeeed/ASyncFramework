using ASyncFramework.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface.Repository
{
    public interface IQueueConfigurationRepository
    {
        Task<bool> Add(QueueConfigurations entity);
        Task<bool> Update(int id, int queueRetry, bool isAutoMapping, int numberOfInstance, byte[] timeStampCheck);
        Task<IEnumerable<QueueConfigurations>> GetAll();
        Task<QueueConfigurations> Get(int id);
    }
}
