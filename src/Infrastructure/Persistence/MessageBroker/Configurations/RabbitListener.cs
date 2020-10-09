using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ASyncFramework.Infrastructure.Persistence.MessageBroker.Configurations
{
    // RabbitListener responsibility: received message from rabbitmq , create queue and exchage and bind queue with exhage 
    public abstract class RabbitListener :  IHostedService
    {
        private readonly IElkLogger<RabbitListener> _logger;
        System.Timers.Timer _timer;
        protected IRabbitMQPersistent _RabbitMQPersistent;
        protected abstract string ExchangeName { get; }
        protected abstract string ExchangeType { get; }
        protected abstract Dictionary<string, object> ExchangeArgu { get; }
        protected abstract string QueueName { get; }
        public RabbitListener(IRabbitMQPersistent rabbitMQPersistent,IElkLogger<RabbitListener> logger)
        {
            _logger = logger;
            _RabbitMQPersistent = rabbitMQPersistent;
        }

        // method called when program booting 
        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                // create exchange , queue and bind queue with exchange
                Register(ExchangeName, QueueName, ExchangeArgu, ExchangeType);
                if (_timer != null) _timer.Dispose();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "failure when try register {CreationDate} {ExchangeName} {QueueName}",
                    DateTime.Now,ExchangeName, QueueName);
                
                //retry to connect 
                _timer = new System.Timers.Timer();
                // 2 minute
                _timer.Interval = 120000;               
                _timer.Elapsed += _timer_Elapsed;
                _timer.Start();
                return Task.CompletedTask;
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            StartAsync(cancellationToken);
            return Task.CompletedTask;
        }
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            StartAsync(new CancellationToken());
        }

        /// <summary>
        /// method resived message from queue 
        /// </summary>
        public abstract Task<bool> Process(string content);

        private void Register(string exchange, string queue, IDictionary<string, object> argu, string exchangeType)
        {            
            // create exchange
            _RabbitMQPersistent.Channel.ExchangeDeclare(exchange, exchangeType, true, false, argu);            
            //create queue 
            _RabbitMQPersistent.Channel.QueueDeclare(queue, true, false, false);
            //bind queue to exchange
            _RabbitMQPersistent.Channel.QueueBind(queue, exchange, queue);

            // register event to resived message from rabbitmq 
            var consumer = new EventingBasicConsumer(_RabbitMQPersistent.Channel);
            _RabbitMQPersistent.Channel.BasicQos(0, 5, true);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.Span);
                bool result = false;
                try
                {
                    _logger.LogReceived(Convert.ToBoolean(JObject.Parse(message)["IsCallBackMessage"]), JObject.Parse(message)["ReferenceNumber"].ToString());

                    result = await Process(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "failure when Process message {CreationDate} {Status} {ExchangeName} {QueueName} {ReferenceNumber}",
                        DateTime.Now,MessageLifeCycle.ExceptoinWhenProcessMessage,ExchangeName, QueueName, JObject.Parse(message)["ReferenceNumber"].ToString());

                    result = false;
                }
                // if process true message will ack 
                if (result)                
                    _RabbitMQPersistent.Channel.BasicAck(ea.DeliveryTag,false);
                else
                    _RabbitMQPersistent.Channel.BasicNack(ea.DeliveryTag, false,true);
            };
            _RabbitMQPersistent.Channel.BasicConsume(queue: queue, consumer: consumer);
        }
    
    }
}
