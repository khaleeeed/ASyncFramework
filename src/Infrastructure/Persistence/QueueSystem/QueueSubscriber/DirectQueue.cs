using ASyncFramework.Domain.Common;
using ASyncFramework.Infrastructure.Persistence.Configurations;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Infrastructure.Persistence.QueueSystem.QueueSubscriber
{
    public class DirectQueue : RabbitListener
    {
        protected override string ExchangeName => "DirectExchange";
        protected override string ExchangeType => "direct";
        protected override Dictionary<string, object> ExchangeArgu => null;
        protected override string QueueName => "DirectQueue";
        public DirectQueue(IOptions<AppConfiguration> options):base(options)
        {
            
        }


        public override bool Process(string message)
        {
            throw new NotImplementedException();
        }
    }
}
