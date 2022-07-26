using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model;
using ASyncFramework.Infrastructure.Persistence.MessageBroker.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
        private readonly ISubscriberLogic _SubscriberLogic;
        
        public DirectQueue(IRabbitMQPersistent rabbitMQPersistent,IInfrastructureLogger<RabbitListener>logger,IConfiguration configuration,INotificationRepository notificationRepository, ISystemRepository systemRepository, IServiceRepository serviceRepository, ISubscriberLogic subscriberLogic) : base(rabbitMQPersistent,logger,configuration,notificationRepository,serviceRepository,systemRepository)
        {
            _RabbitMQPersistent.ConnectionName = ExchangeName;
            _SubscriberLogic = subscriberLogic;
        }

        public override async Task<bool> Process(Message message)
        {
            await _SubscriberLogic.Subscribe(message);
            return true;
        }

        public override async Task InternalExceptionRetry(Message message)
        {
            await _SubscriberLogic.InternalExceptionRetry(message);
        }
    }
}