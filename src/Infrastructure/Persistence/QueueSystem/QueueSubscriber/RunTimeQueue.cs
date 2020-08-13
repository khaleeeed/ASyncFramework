using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Infrastructure.Persistence.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.QueueSystem.QueueSubscriber
{
    public class RunTimeQueue : IHostedService
    {
        private readonly List<AutoMappingDelayQueue> _autoMappingDelayQueues = new List<AutoMappingDelayQueue>();

        public RunTimeQueue(IServiceProvider serviceProvider,IOptions<Dictionary<string,QueueConfiguration>> queueConfigurations,IElkLogger<RabbitListener>logger)
        {
             var runTimeQueue = queueConfigurations.Value.Where(x => x.Value.IsAutoMapping).Select(x => x.Value);
            foreach (var queue in runTimeQueue)
            {
                _autoMappingDelayQueues.Add(new AutoMappingDelayQueue(serviceProvider.GetService<IRabbitMQPersistent>(), serviceProvider.GetService<ISubscriberLogic>(), queue,logger));
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var autoMappingDelayQueue in _autoMappingDelayQueues)
            {
                autoMappingDelayQueue.StartAsync(cancellationToken);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var autoMappingDelayQueue in _autoMappingDelayQueues)
            {
                autoMappingDelayQueue.StopAsync(cancellationToken);
            }
            return Task.CompletedTask;
        }
    }
}
