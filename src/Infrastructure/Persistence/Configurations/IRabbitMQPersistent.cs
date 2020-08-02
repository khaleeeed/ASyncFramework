using RabbitMQ.Client;
using System;

namespace ASyncFramework.Infrastructure.Persistence.Configurations
{
    public interface IRabbitMQPersistent: IDisposable
    {
        IModel Channel { get; }
        IConnection Connection { get; }
        bool IsConnected { get; }
    }
}