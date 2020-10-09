using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using ASyncFramework.Infrastructure.Persistence.MessageBroker.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.MessageBroker.QueueSystem.QueueSubscriber
{
    // RabbitListener register as hosted service 
    // HostedService life time is Singleton
    // responsibility received message from rabbitmq and call logic 
    // DirectQueue for send message immediately
    public class DirectQueue : RabbitListener
    {
        protected override string ExchangeName => "DirectExchange";
        protected override string ExchangeType => "direct";
        protected override Dictionary<string, object> ExchangeArgu => null;
        protected override string QueueName => "DirectQueue";
       
        private readonly ISubscriberLogic _subscriberLogic;
        public DirectQueue(IRabbitMQPersistent rabbitMQPersistent, ISubscriberLogic subscriberLogic,IElkLogger<RabbitListener>logger) : base(rabbitMQPersistent,logger)
        {
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
