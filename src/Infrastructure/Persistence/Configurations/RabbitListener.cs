using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ASyncFramework.Infrastructure.Persistence.Configurations
{
    public abstract class RabbitListener :  IHostedService ,IDisposable
    {
        private IElkLogger<RabbitListener> _logger;
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
            _RabbitMQPersistent.RetryToRegisterChannelEvent += StartAsync;            
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Register(ExchangeName, QueueName, ExchangeArgu, ExchangeType);
                if (_timer != null) _timer.Dispose();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "failure when try register {ExchangeName} {QueueName}",
                    ExchangeName, QueueName);
                
                // retry to connect 
                _timer = new System.Timers.Timer();
                _timer.Interval = 600000;
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

        public abstract Task<bool> Process(string content);
        private void Register(string exchange, string queue, IDictionary<string, object> argu, string exchangeType)
        {
            _RabbitMQPersistent.Channel.ExchangeDeclare(exchange, exchangeType, true, false, argu);
            _RabbitMQPersistent.Channel.QueueDeclare(queue, true, false, false);
            _RabbitMQPersistent.Channel.QueueBind(queue, exchange, queue);

            var consumer = new EventingBasicConsumer(_RabbitMQPersistent.Channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.Span);
                bool result = false;
                try
                {
                    _logger.LogReceived(Convert.ToBoolean(JObject.Parse(message)["IsCallBackMessage"]), message, JObject.Parse(message)["ReferenceNumber"].ToString());

                    result = await Process(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "failure when Process message {ExchangeName} {QueueName} {ReferenceNumber}",
                        ExchangeName, QueueName, JObject.Parse(message)["ReferenceNumber"].ToString());

                    result = false;
                }
                if (result)
                {
                    _RabbitMQPersistent.Channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            _RabbitMQPersistent.Channel.BasicConsume(queue: queue, consumer: consumer);
        }

        public void Dispose()
        {
            _RabbitMQPersistent.Dispose();
        }
    }
}
