using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using ASyncFramework.Infrastructure.Persistence.Configurations;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Infrastructure.Persistence.QueueSystem.QueueSubscriber
{
    public class FiveMinuteQueue : RabbitListener
    {
        protected override string ExchangeName => "FiveMinuteExchange";
        protected override string ExchangeType => "x-delayed-message";
        protected override Dictionary<string, object> ExchangeArgu => new Dictionary<string, object>{{"x-delayed-type", "direct"}};
        protected override string QueueName => "FiveMinuteQueue";

        private readonly ISubscriberLogic _subscriberLogic;
        public FiveMinuteQueue(IRabbitMQPersistent rabbitMQPersistent, ISubscriberLogic subscriberLogic) : base(rabbitMQPersistent)
        {
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
