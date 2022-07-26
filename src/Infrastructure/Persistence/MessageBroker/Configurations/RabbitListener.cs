using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ASyncFramework.Domain.Enums;
using ASyncFramework.Domain.Interface;
using ASyncFramework.Domain.Interface.Repository;
using ASyncFramework.Domain.Model;
using Elastic.Apm;
using Elastic.Apm.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ASyncFramework.Infrastructure.Persistence.MessageBroker.Configurations
{
    // RabbitListener responsibility: received message from rabbitmq , create queue and exchage and bind queue with exhage 
    public abstract class RabbitListener :  IHostedService
    {
        public bool IsRunning { get; set; } = true;
        private readonly IInfrastructureLogger<RabbitListener> _logger;
        private readonly INotificationRepository _NotificationRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly ISystemRepository _systemRepository;

        System.Timers.Timer _timer;
        protected IRabbitMQPersistent _RabbitMQPersistent;
        protected readonly string _SubscribeUrl;

        protected abstract string ExchangeName { get; }
        protected abstract string ExchangeType { get; }
        protected abstract Dictionary<string, object> ExchangeArgu { get; }
        protected abstract string QueueName { get; }

        public RabbitListener(IRabbitMQPersistent rabbitMQPersistent,IInfrastructureLogger<RabbitListener> logger,IConfiguration configuration, INotificationRepository notificationRepository, IServiceRepository  serviceRepository, ISystemRepository systemRepository)
        {
            _logger = logger;
            _RabbitMQPersistent = rabbitMQPersistent;
            _SubscribeUrl = configuration.GetValue<string>("SubscribeUrl");
            //_DisposeRepository = disposeRepository;
            _NotificationRepository = notificationRepository;
            _serviceRepository = serviceRepository;
            _systemRepository = systemRepository;
        }

        // method called when program booting 
        public Task StartAsync(CancellationToken cancellationToken)
        {                       
            try
            {
                IsRunning = true;
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
            _RabbitMQPersistent?.Channel?.Dispose();
            IsRunning = false;
            return Task.CompletedTask;
        }
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            StartAsync(new CancellationToken());
        }

        /// <summary>
        /// method resived message from queue 
        /// </summary>
        public abstract Task<bool> Process(Message message);
        /// <summary>
        /// method retry if message
        /// </summary>               
        public abstract Task InternalExceptionRetry(Message content);

        private void Register(string exchange, string queue, IDictionary<string, object> argu, string exchangeType)
        {
            // create exchange
            _RabbitMQPersistent.Channel.ExchangeDeclare(exchange, exchangeType, true, false, argu);
            //create queue 
            _RabbitMQPersistent.Channel.QueueDeclare(queue, true, false, false);
            //bind queue to exchange
            _RabbitMQPersistent.Channel.QueueBind(queue, exchange, queue);

            // register event to resived message from rabbitmq 
            EventingBasicConsumer consumer = new EventingBasicConsumer(_RabbitMQPersistent.Channel);
            _RabbitMQPersistent.Channel.BasicQos(0, 1, true);

            consumer.Received += async (model, ea) =>
            {

                await Agent.Tracer
                     .CaptureTransaction("ConsumeMessage", ApiConstants.TypeRequest, async (t) =>
                     {
                         var body = ea.Body;
                         var content = Encoding.UTF8.GetString(body.Span);
                         bool result = false;
                         var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(content);

                         try
                         {
                             Agent.Tracer.CurrentTransaction.SetLabel("ReferenceNumber", message.ReferenceNumber);

                             _logger.LogReceived(DateTime.Now, message.IsCallBackMessage, message.ReferenceNumber);

                             // stop dispose funcation
                             //var disposeModel = await _DisposeRepository.FindDocument(referenceNumber);
                             //if (disposeModel != null)
                             //{
                             //    _logger.LogFinish(DateTime.Now, MessageLifeCycle.MessageDisposed, referenceNumber);
                             //    await _NotificationRepository.UpdateStatusId(referenceNumber, MessageLifeCycle.MessageDisposed);
                             //    _RabbitMQPersistent.Channel.BasicAck(ea.DeliveryTag, false);
                             //    return;
                             //}

                             // check if request dispose
                             if (!_systemRepository.CheckSystemActive(message.SystemCode).isActive || !_serviceRepository.CheckServiceActive(message.ServiceCode))
                             {
                                 _logger.LogFinish(DateTime.Now, MessageLifeCycle.MessageDisposed, message.ReferenceNumber);
                                 _NotificationRepository.UpdateStatusId(message.ReferenceNumber, MessageLifeCycle.MessageDisposed);
                                 _RabbitMQPersistent.Channel.BasicAck(ea.DeliveryTag, false);
                                 return;
                             }

                             result = await Process(message);
                         }
                         catch (Exception ex)
                         {
                             _logger.LogError(ex, "failure when Process message {CreationDate} {Status} {ExchangeName} {QueueName} {ReferenceNumber}",
                                 DateTime.Now, MessageLifeCycle.ExceptoinWhenProcessMessage, ExchangeName, QueueName, message.ReferenceNumber);
                             _ = InternalExceptionRetry(message);
                             result = false;
                             Agent.Tracer.CurrentTransaction.Outcome = Outcome.Failure;
                             Agent.Tracer.CurrentTransaction.Result = "500";
                         }

                         // if process true message will ack 
                         _RabbitMQPersistent.Channel.BasicAck(ea.DeliveryTag, false);
                     });
            };

            _RabbitMQPersistent.Channel.BasicConsume(queue: queue, consumer: consumer);
        }
    }
}