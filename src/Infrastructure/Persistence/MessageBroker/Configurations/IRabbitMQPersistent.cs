using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.MessageBroker.Configurations
{
    
    public interface IRabbitMQPersistent: IPooledObjectPolicy<IModel>
    {
        IModel Channel { get; }        
        bool IsConnected { get; }
        string ConnectionName { get; set; }
    }
}