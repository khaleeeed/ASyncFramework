using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
using ASyncFramework.Infrastructure.Persistence.Configurations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASyncFramework.Infrastructure.Persistence.QueueSystem
{
    public class RabbitProducers : IRabbitProducers
    {
        protected IRabbitMQPersistent  _RabbitMQPersistent;
        public RabbitProducers(IRabbitMQPersistent rabbitMQPersistent)
        {
            _RabbitMQPersistent = rabbitMQPersistent;
        }

        public virtual void PushMessage(Message message,QueueConfiguration queueConfiguration)
        {
            string msgJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(msgJson);

            IBasicProperties properties = _RabbitMQPersistent.Channel.CreateBasicProperties();
            var headers = new Dictionary<string, object>();
            properties.Persistent = true;
            properties.DeliveryMode = 2;
            headers.Add("x-delay", queueConfiguration.Dealy);
            properties.Headers = headers;
            _RabbitMQPersistent.Channel.BasicPublish(exchange: queueConfiguration.ExhangeName,
                                 routingKey: queueConfiguration.QueueName,
                                 basicProperties: properties,
                                 body: body);
        }

        public void Dispose()
        {
            _RabbitMQPersistent.Dispose();
        }


    }
}
