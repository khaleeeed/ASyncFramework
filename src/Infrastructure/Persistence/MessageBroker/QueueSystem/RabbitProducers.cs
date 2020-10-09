using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Infrastructure.Persistence.MessageBroker.QueueSystem
{
    // RabbitListener register as hosted service 
    // responsibility push message to rabbitmq 
    public class RabbitProducers : IRabbitProducers
    {
        protected readonly DefaultObjectPool<IModel> _RabbitMQPersistent;
        private IElkLogger<RabbitProducers> _logger;
        public RabbitProducers(IPooledObjectPolicy<IModel> rabbitMQPersistent,IElkLogger<RabbitProducers> logger)
        {
            _RabbitMQPersistent =  new DefaultObjectPool<IModel>(rabbitMQPersistent, 1500);
            _logger = logger;
        }

        public virtual void PushMessage(Message message, QueueConfiguration queueConfiguration)
        {
            string msgJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(msgJson);
            var channel = _RabbitMQPersistent.Get();
            try
            {
                IBasicProperties properties = channel.CreateBasicProperties();
                var headers = new Dictionary<string, object>();
                properties.Persistent = true;
                properties.DeliveryMode = 2;
                headers.Add("x-delay", queueConfiguration.Dealy);
                properties.Headers = headers;

                channel.BasicPublish(exchange: queueConfiguration.ExhangeName,
                                     routingKey: queueConfiguration.QueueName,
                                     basicProperties: properties,
                                     body: body);

                _logger.LogPublishing(message);
            }
            finally
            {
                _RabbitMQPersistent.Return(channel);
            }

        }        


    }
}
