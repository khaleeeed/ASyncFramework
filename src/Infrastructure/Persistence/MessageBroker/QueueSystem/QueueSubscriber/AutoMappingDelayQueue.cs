using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using ASyncFramework.Infrastructure.Persistence.MessageBroker.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.MessageBroker.QueueSystem.QueueSubscriber
{
    // RabbitListener register as hosted service 
    // HostedService life time is Singleton
    // responsibility received message from rabbitmq and call logic 
    // AutoMappingDelayQueue for adding queue from appsettings 
    public class AutoMappingDelayQueue: RabbitListener
    {
        protected override string ExchangeName => _queueConfiguration.ExhangeName;
        protected override string ExchangeType => "x-delayed-message";
        protected override Dictionary<string, object> ExchangeArgu => new Dictionary<string, object> { { "x-delayed-type", "direct" } };
        protected override string QueueName => _queueConfiguration.QueueName;
        QueueConfiguration _queueConfiguration;
        private readonly ISubscriberLogic _subscriberLogic;
        public AutoMappingDelayQueue(IRabbitMQPersistent rabbitMQPersistent, ISubscriberLogic subscriberLogic, QueueConfiguration queueConfiguration,IElkLogger<RabbitListener>logger) : base(rabbitMQPersistent, logger)
        {
            _queueConfiguration = queueConfiguration;
            _subscriberLogic = subscriberLogic;
            _RabbitMQPersistent.ConnectionName = ExchangeName;

        }

        public override async Task<bool> Process(string content)
        {
            var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(content);
            await _subscriberLogic.Subscribe(message);
            return true;
        }
    }
}