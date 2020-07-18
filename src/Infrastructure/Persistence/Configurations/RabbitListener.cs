using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ASyncFramework.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace ASyncFramework.Infrastructure.Persistence.Configurations
{
    public abstract class RabbitListener : IHostedService ,IDisposable
    {
        System.Timers.Timer _timer;
        private readonly ConnectionFactory _factory;
        private  IConnection _connection;
        private IModel _channel;
        protected IConnection Connection => _connection ??= _factory.CreateConnection();
        protected IModel Channel => _channel ??= Connection.CreateModel();
        protected abstract string ExchangeName { get; }
        protected abstract string ExchangeType { get; }
        protected abstract Dictionary<string, object> ExchangeArgu { get; }
        protected abstract string QueueName { get; }
        public RabbitListener(IOptions<AppConfiguration> options)
        {
            _factory = new ConnectionFactory()
            {
                HostName = options.Value.RabbitHost,
                UserName = options.Value.RabbitUserName,
                Password = options.Value.RabbitPassword,
            };
            
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
                // log exc

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
            _connection.Close();
            return Task.CompletedTask;
        }
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            StartAsync(new CancellationToken());
        }

        public abstract bool Process(string content);
        public void Register(string exchange, string queue, IDictionary<string, object> argu, string exchangeType)
        {
            Channel.ExchangeDeclare(exchange, exchangeType, true, false, argu);
            Channel.QueueDeclare(queue, true, false, false);
            Channel.QueueBind(queue, exchange, queue);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.Span);
                var result = Process(message);
                if (result)
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            _channel.BasicConsume(queue: queue, consumer: consumer);
        }
        public void DeRegister()
        {
            _connection.Close();
        }

        public void Dispose()
        {
            _channel.Dispose();
        }
    }
}
