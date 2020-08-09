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
    public class RabbitMQPersistent : IRabbitMQPersistent
    {
        private bool IsFailureConnection;
        private System.Timers.Timer _timer;
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;
        public IConnection Connection { get { if (_connection != null) { return _connection; } _connection = _factory.CreateConnection(); Connect(); return _connection; } }
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

        public event Func<CancellationToken,Task> RetryToRegisterChannelEvent;

        public void Connect()
        {
            if (IsConnected)
            {
                _connection.ConnectionShutdown += OnConnectionShutdown;
                _connection.CallbackException += OnCallbackException;
                _connection.ConnectionBlocked += OnConnectionBlocked;
                if (IsFailureConnection)
                    RetryToRegisterChannelEvent?.Invoke(CancellationToken.None);
                IsFailureConnection = false;
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            // log 

            // retry connect 
            if (_timer==null)
            {
                _timer = new System.Timers.Timer();
                _timer.Interval = 600000;
                _timer.Elapsed += RetryToConnect; ;
                _timer.Start();
            }
            IsFailureConnection = true;
        }    

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            // log 

            // retry connect 
            if (_timer == null)
            {
                _timer = new System.Timers.Timer();
                _timer.Interval = 600000;
                _timer.Elapsed += RetryToConnect; ;
                _timer.Start();
            }
            IsFailureConnection = true;
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            // log 


            // retry connect 
            if (_timer == null)
            {
                _timer = new System.Timers.Timer();
                _timer.Interval = 600000;
                _timer.Elapsed += RetryToConnect; ;
                _timer.Start();
            }
            IsFailureConnection = true;
        }
        private void RetryToConnect(object sender, ElapsedEventArgs e)
        {
            // dispose connection 
            DisposeConnection();

            try
            {
                _connection = _factory.CreateConnection();

            }
            catch (Exception ex)
            {
                // log 
            }
            if (IsConnected)
            {
                _timer?.Dispose();
                _timer = null;
                Connect();
            }
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