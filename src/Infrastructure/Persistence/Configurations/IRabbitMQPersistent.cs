using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.Configurations
{
    public interface IRabbitMQPersistent: IDisposable
    {
        IModel Channel { get; }
        IConnection Connection { get; }
        bool IsConnected { get; }
        event Func<CancellationToken,Task> RetryToRegisterChannelEvent;
    }
}