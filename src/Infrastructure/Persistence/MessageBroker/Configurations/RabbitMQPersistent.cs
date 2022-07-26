using ASyncFramework.Domain.Common;
using ASyncFramework.Domain.Interface;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ASyncFramework.Infrastructure.Persistence.MessageBroker.Configurations
{
    // RabbitMQPersistent register as transient     
    // responsibility manage rabbitmq connection and channel    
    public class RabbitMQPersistent : IRabbitMQPersistent
    {
        public string ConnectionName { get; set; } = null;       
        private readonly ConnectionFactory _factory;
        private readonly IInfrastructureLogger<RabbitMQPersistent> _logger;
        private readonly IGetIPAddress _IPAddressService;
        private IConnection _connection;
        private IModel _channel;
        private int _numberOfChannel=0;
        public IConnection Connection
        {
            get
            {                
                if (IsConnected )
                    return _connection;
               _connection = _factory.CreateConnection($"{_IPAddressService.LocalIpAddress}-{ConnectionName??"Producers"}");
                Connect();
                return _connection;
            }
        }
        public IModel Channel 
        {
            get
            {
                if (!IsChannelConnected)
                    _channel = Connection.CreateModel();

                return _channel;
            }
        }
        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen;
            }
        }
        public bool IsChannelConnected
        {
            get
            {
                return _channel != null && _channel.IsOpen; 
            }
        }

        public RabbitMQPersistent(IGetIPAddress IPAddressService, IOptionsMonitor<AppConfiguration> options,IInfrastructureLogger<RabbitMQPersistent> logger)
        {
            _IPAddressService = IPAddressService;
            _logger = logger;

            AppConfiguration appConfiguration = options.Get(AppConfiguration.RabbitMq);
            _factory = new ConnectionFactory()
            {
                HostName = appConfiguration.Host,
                UserName = appConfiguration.UserName,
                Password = appConfiguration.Password,
            };            
        }

        public void Connect()
        {
            if (IsConnected)
            {
                // register event 
                _connection.ConnectionShutdown += OnConnectionShutdown;
                _connection.CallbackException += OnCallbackException;
                _connection.ConnectionBlocked += OnConnectionBlocked;
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            _logger.LogError("ConnectionBlocked {CreationDate} {e}", DateTime.Now,Newtonsoft.Json.JsonConvert.SerializeObject(e));
        }    

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            _logger.LogError("ConnectionCallbackException {CreationDate} {e}", DateTime.Now,Newtonsoft.Json.JsonConvert.SerializeObject(e));

        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogError("ConnectionShutdown {CreationDate} {e}", DateTime.Now,Newtonsoft.Json.JsonConvert.SerializeObject(e));
        }


        // pooling methods manage channel in publisher   
        public IModel Create()
        {
            _numberOfChannel++;
            return Connection.CreateModel();
        }
        public bool Return(IModel obj)
        {
            if (obj.IsOpen)
            {
                return true;
            }
            else
            {
                _numberOfChannel--;
                obj?.Dispose();
                return false;
            }
        }
    }
}