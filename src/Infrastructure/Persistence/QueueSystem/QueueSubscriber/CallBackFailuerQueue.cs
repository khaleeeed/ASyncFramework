using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using ASyncFramework.Infrastructure.Persistence.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASyncFramework.Infrastructure.Persistence.QueueSystem.QueueSubscriber
{
    public class CallBackFailuerQueue : RabbitListener
    {
        protected override string ExchangeName => "CallBackFailuerExchange";
        protected override string ExchangeType => "x-delayed-message";
        protected override Dictionary<string, object> ExchangeArgu => new Dictionary<string, object> { { "x-delayed-type", "direct" } };
        protected override string QueueName => "CallBackFailuerQueue";
        
        private readonly ISubscriberLogic _subscriberLogic;
        public CallBackFailuerQueue(IRabbitMQPersistent rabbitMQPersistent,ISubscriberLogic subscriberLogic,IElkLogger<RabbitListener>logger) : base(rabbitMQPersistent,logger)
        {
            _subscriberLogic = subscriberLogic;           
        }

        public override async Task<bool> Process(string content)
        {
            var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(content); 
            await _subscriberLogic.Subscribe(message);
            return true;
        }
    }
}
