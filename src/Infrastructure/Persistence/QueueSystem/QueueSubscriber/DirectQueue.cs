using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using ASyncFramework.Infrastructure.Persistence.Configurations;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.QueueSystem.QueueSubscriber
{
    public class DirectQueue : RabbitListener
    {
        protected override string ExchangeName => "DirectExchange";
        protected override string ExchangeType => "direct";
        protected override Dictionary<string, object> ExchangeArgu => null;
        protected override string QueueName => "DirectQueue";
       
        private readonly ISubscriberLogic _subscriberLogic;
        public DirectQueue(IRabbitMQPersistent rabbitMQPersistent, ISubscriberLogic subscriberLogic) : base(rabbitMQPersistent)
        {
            _subscriberLogic = subscriberLogic;
        }

        public override async Task<bool> Process(string content)
        {
            var message = System.Text.Json.JsonSerializer.Deserialize<Message>(content);
            await _subscriberLogic.Subscribe(message);
            return true;
        }
    }
}
