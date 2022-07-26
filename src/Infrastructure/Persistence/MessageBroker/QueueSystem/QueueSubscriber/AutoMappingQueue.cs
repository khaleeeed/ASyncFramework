using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model;
using ASyncFramework.Infrastructure.Persistence.MessageBroker.Configurations;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.MessageBroker.QueueSystem.QueueSubscriber
{
    // RabbitListener register as hosted service 
    // HostedService life time is Singleton
    // responsibility received message from rabbitmq and call logic 
    // AutoMappingDelayQueue for adding queue from appsettings 
    public class AutoMappingQueue: RabbitListener
    {
        protected override string ExchangeName => _queueConfiguration.ExhangeName;
        protected override string ExchangeType => _queueConfiguration.ExhangeType;
        protected override Dictionary<string, object> ExchangeArgu => new Dictionary<string, object> { { "x-delayed-type", "direct" } };
        protected override string QueueName => _queueConfiguration.QueueName;
        private readonly QueueConfigurations _queueConfiguration;
        private readonly ISubscriberLogic _SubscriberLogic;
        public AutoMappingQueue(IRabbitMQPersistent rabbitMQPersistent, QueueConfigurations queueConfiguration,
            IInfrastructureLogger<RabbitListener>logger,IConfiguration configuration,/*IDisposeRepository disposeRepository,*/
            INotificationRepository notificationRepository, ISystemRepository systemRepository, IServiceRepository serviceRepository, ISubscriberLogic subscriberLogic) : base(rabbitMQPersistent, logger,configuration,notificationRepository,serviceRepository,systemRepository)
        {
            _queueConfiguration = queueConfiguration;
            _RabbitMQPersistent.ConnectionName = ExchangeName;
            _SubscriberLogic = subscriberLogic;

        }

        public override async Task<bool> Process(Message message)
        {
            await _SubscriberLogic.Subscribe(message);
            return true;
        }

        public override async Task InternalExceptionRetry(Message message)
        {
            await _SubscriberLogic.InternalExceptionRetry(message);
        }
    }
}