using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using ASyncFramework.Infrastructure.Persistence.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Infrastructure.Persistence.QueueSystem.QueueSubscriber
{
    public class AutoMappingDelayQueue: RabbitListener
    {
        protected override string ExchangeName => _queueConfiguration.ExhangeName;
        protected override string ExchangeType => "x-delayed-message";
        protected override Dictionary<string, object> ExchangeArgu => new Dictionary<string, object> { { "x-delayed-type", "direct" } };
        protected override string QueueName => _queueConfiguration.QueueName;
        QueueConfiguration _queueConfiguration;
        private readonly ISubscriberLogic _subscriberLogic;
        public AutoMappingDelayQueue(IRabbitMQPersistent rabbitMQPersistent, ISubscriberLogic subscriberLogic, QueueConfiguration queueConfiguration) : base(rabbitMQPersistent)
        {
            _queueConfiguration = queueConfiguration;
            _subscriberLogic = subscriberLogic;
        }

        public override bool Process(string content)
        {
            var message = System.Text.Json.JsonSerializer.Deserialize<Message>(content);
            _subscriberLogic.Subscribe(message);
            return true;
        }
    }
}