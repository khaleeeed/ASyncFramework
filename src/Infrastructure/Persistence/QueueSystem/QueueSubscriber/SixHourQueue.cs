using ASyncFramework.Domain.Common;
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
        public SixHourQueue(IOptions<AppConfiguration> options) : base(options)
        {

        }


        public override bool Process(string message)
        {
            throw new NotImplementedException();
        }
    }
}

