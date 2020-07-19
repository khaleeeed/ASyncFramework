using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Model;
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
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;
        protected IConnection Connection => _connection ??= _factory.CreateConnection();
        protected IModel Channel => _channel ??= Connection.CreateModel();
        public RabbitProducers(IOptions<AppConfiguration> options)
        {
            _factory = new ConnectionFactory()
            {
                HostName = options.Value.RabbitHost,
                UserName = options.Value.RabbitUserName,
                Password = options.Value.RabbitPassword,
            };

        }

        public virtual void PushMessage(Message message,QueueConfiguration queueConfiguration)
        {
            string msgJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(msgJson);

            IBasicProperties properties = Channel.CreateBasicProperties();
            var headers = new Dictionary<string, object>();
            properties.Persistent = true;
            properties.DeliveryMode = 2;
            headers.Add("x-delay", queueConfiguration.Dealy);
            properties.Headers = headers;
            Channel.BasicPublish(exchange: queueConfiguration.ExhangeName,
                                 routingKey: queueConfiguration.QueueName,
                                 basicProperties: properties,
                                 body: body);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _channel = null;
        }


    }
}
