using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Infrastructure.Persistence.MessageBroker.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.MessageBroker.QueueSystem.QueueSubscriber
{
    // RabbitListener register as hosted service 
    // HostedService life time is Singleton
    // responsibility received message from rabbitmq and call logic 
    // RunTimeQueue for create message from appsettings
    public class RunTimeQueue : IHostedService
    {
        private readonly List<AutoMappingQueue> _autoMappingDelayQueues = new List<AutoMappingQueue>();
        public bool IsRunning { get; private set; } = true;
        private readonly IQueueConfigurationService _QueueConfigurations;
        private readonly IServiceProvider _ServiceProvider;
        private readonly IInfrastructureLogger<RabbitListener> _Logger;
        private readonly ISubscriberRepository _SubscriberRepository;
        private readonly ISystemRepository _SystemRepository;
        public RunTimeQueue(IServiceProvider serviceProvider,ISystemRepository systemRepository, IQueueConfigurationService queueConfigurations,IInfrastructureLogger<RabbitListener>logger, ISubscriberRepository subscriberRepository)
        {
            _QueueConfigurations = queueConfigurations;
            _ServiceProvider = serviceProvider;
            _Logger = logger;
            _SubscriberRepository = subscriberRepository;
            _SystemRepository = systemRepository;
            CreateInstance();
            _SubscriberRepository.AddOrUpdate(true);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            IsRunning = true;
            foreach (var autoMappingDelayQueue in _autoMappingDelayQueues)
            {
                autoMappingDelayQueue.StartAsync(cancellationToken);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            IsRunning = false;
            foreach (var autoMappingDelayQueue in _autoMappingDelayQueues)
            {
                autoMappingDelayQueue.StopAsync(cancellationToken);
            }
            return Task.CompletedTask;
        }

        public void ReCreateInstance()
        {
            CreateInstance();
        }

        private void CreateInstance()
        {
            _autoMappingDelayQueues.Clear();
            // get all queues from database 
            // is automapping == isActive 
            IEnumerable<QueueConfigurations> runTimeQueue = _QueueConfigurations.QueueConfiguration.Where(x => x.Value.IsAutoMapping).Select(x => x.Value);
            // create default queues
            foreach (var queue in runTimeQueue)
            {
                for (int i = 0; i < queue.NumberOfInstance; i++)
                {
                    _autoMappingDelayQueues.Add(new AutoMappingQueue(_ServiceProvider.GetService<IRabbitMQPersistent>(), queue, _Logger, _ServiceProvider.GetService<IConfiguration>(), _ServiceProvider.GetService<INotificationRepository>(), _ServiceProvider.GetService<ISystemRepository>(), _ServiceProvider.GetService<IServiceRepository>(), _ServiceProvider.GetService<ISubscriberLogic>()));
                }
            }

            var systems= _SystemRepository.GetAll().Result;
            foreach (var system in systems)
            {
                if (system.HasCustomQueue)
                {
                    foreach (var queue in runTimeQueue)
                    {
                        var systemQueue = new QueueConfigurations
                        {
                            Dealy = queue.Dealy,
                            ExhangeName = queue.ExhangeName,
                            QueueName = $"{queue.QueueName}_{system.SystemCode}",
                            ExhangeType=queue.ExhangeType,
                            QueueRetry=queue.QueueRetry                            
                        };
                    
                        for (int i = 0; i < queue.NumberOfInstance; i++)
                        {
                            _autoMappingDelayQueues.Add(new AutoMappingQueue(_ServiceProvider.GetService<IRabbitMQPersistent>(), systemQueue, _Logger, _ServiceProvider.GetService<IConfiguration>(), _ServiceProvider.GetService<INotificationRepository>(), _ServiceProvider.GetService<ISystemRepository>(), _ServiceProvider.GetService<IServiceRepository>(), _ServiceProvider.GetService<ISubscriberLogic>()));
                        }
                    }
                }
            }
        }
    }
}