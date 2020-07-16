using ASyncFramework.Domain.Common;
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
        public FiveMinuteQueue(IOptions<AppConfiguration> options) : base(options)
        {

        }


        public override bool Process(string message)
        {
            throw new NotImplementedException();
        }
    }
}
