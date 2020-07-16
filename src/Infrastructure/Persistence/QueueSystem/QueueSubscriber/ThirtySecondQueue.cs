using ASyncFramework.Domain.Common;
using ASyncFramework.Infrastructure.Persistence.Configurations;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Infrastructure.Persistence.QueueSystem.QueueSubscriber
{
    public class ThirtySecondQueue : RabbitListener
    {
        protected override string ExchangeName => "ThirtySecondExchange";
        protected override string ExchangeType => "x-delayed-message";
        protected override Dictionary<string, object> ExchangeArgu => new Dictionary<string, object> { { "x-delayed-type", "direct" } };
        protected override string QueueName => "ThirtySecondQueue";
        public ThirtySecondQueue(IOptions<AppConfiguration> options) : base(options)
        {

        }

        public override bool Process(string message)
        {
            throw new NotImplementedException();
        }
    }
}

