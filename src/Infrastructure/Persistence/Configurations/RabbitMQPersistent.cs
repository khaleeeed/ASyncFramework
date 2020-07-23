using ASyncFramework.Domain.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ASyncFramework.Infrastructure.Persistence.Configurations
{
    public class RabbitMQPersistent : IDisposable
    {
        private System.Timers.Timer _timer;
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;
        public IConnection Connection { get { _connection ??= _factory.CreateConnection(); Connect(); return _connection; } }
        public IModel Channel => _channel ??= Connection.CreateModel();
        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen;
            }
        }

        public RabbitMQPersistent(IOptions<AppConfiguration> options)
        {
            _factory = new ConnectionFactory()
            {
                HostName = options.Value.RabbitHost,
                UserName = options.Value.RabbitUserName,
                Password = options.Value.RabbitPassword,
            };

        }

        public void Connect()
        {
            if (IsConnected)
            {
                _connection.ConnectionShutdown += OnConnectionShutdown;
                _connection.CallbackException += OnCallbackException;
                _connection.ConnectionBlocked += OnConnectionBlocked;
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            // log 

            // dispose connection 
            DisposeConnection();

            // retry connect 
            _=Connection;

        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            // log 

            // dispose connection 
            DisposeConnection();

            // retry connect 
            _ = Connection;
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            // log 

            // dispose connection 
            DisposeConnection();

            // retry connect 
            _ = Connection;
        }

        private void DisposeConnection()
        {
            _connection?.Dispose();
            _connection = null;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _channel = null;
        }

    }
}
