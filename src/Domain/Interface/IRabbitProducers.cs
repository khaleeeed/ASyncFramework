using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Model;
using System;
using System.Threading.Tasks;

namespace ASyncFramework.Domain.Interface
{
    public interface IRabbitProducers:IDisposable
    {
        void PushMessage(Message message, QueueConfiguration queueConfiguration);
    }
}