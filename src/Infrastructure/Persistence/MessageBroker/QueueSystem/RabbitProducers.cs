using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model;
using Microsoft.Extensions.ObjectPool;
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
        private IInfrastructureLogger<RabbitProducers> _logger;
        private readonly IPushFailuerRepository _pushFailuerRepository;
        private readonly INotificationRepository _repository;


        public RabbitProducers(IPooledObjectPolicy<IModel> rabbitMQPersistent,IInfrastructureLogger<RabbitProducers> logger, IPushFailuerRepository pushFailuerRepository, INotificationRepository repository)
        {
            _RabbitMQPersistent =  new DefaultObjectPool<IModel>(rabbitMQPersistent, 1500);
            _logger = logger;
            _pushFailuerRepository = pushFailuerRepository;
            _repository = repository;
        }

        public virtual void PushMessage(Message message, QueueConfigurations queueConfiguration)
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
                                     routingKey: message.HasCustomQueue ? $"{queueConfiguration.QueueName}_{message.SystemCode}" : queueConfiguration.QueueName,
                                     basicProperties: properties,
                                     body: body);

                _logger.LogPublishing(DateTime.Now, message);
            }
            catch(Exception ex)
            {
                _pushFailuerRepository.Add(new Domain.Entities.PushFailuerEntity { CreationDate = DateTime.Now, IsActive = true, NotificationId = Convert.ToInt64(message.ReferenceNumber) });
                _logger.LogError(DateTime.Now, ex, MessageLifeCycle.FailurePushToQueue, message.ReferenceNumber);
                _repository.UpdateStatusId(message.ReferenceNumber, MessageLifeCycle.FailurePushToQueue);
                throw;
            }
            finally
            {
                _RabbitMQPersistent.Return(channel);
            }
        }
    }
}