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
    public class SixHourQueue : RabbitListener
    {
        protected override string ExchangeName => "SixHourExchange";
        protected override string ExchangeType => "x-delayed-message";
        protected override Dictionary<string, object> ExchangeArgu => new Dictionary<string, object> { { "x-delayed-type", "direct" } };
        protected override string QueueName => "SixHourQueue";
       
        private readonly ISubscriberLogic _subscriberLogic;
        public SixHourQueue(IOptions<AppConfiguration> options, ISubscriberLogic subscriberLogic) : base(options)
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

