using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Messaging.Configuration;
using System;
using System.Threading;

namespace Shared.Messaging.Infrastructure
{
    public class RabbitMQConnection : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory; // Changed from IConnectionFactory to ConnectionFactory
        private readonly ILogger<RabbitMQConnection> _logger;
        private readonly int _retryCount;
        private IConnection? _connection;
        private bool _disposed;
        private readonly object _syncRoot = new();

        public RabbitMQConnection(
            IOptions<RabbitMQSettings> settings,
            ILogger<RabbitMQConnection> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryCount = settings.Value.ConnectionRetryCount;
            
            _connectionFactory = new ConnectionFactory
            {
                HostName = settings.Value.HostName,
                UserName = settings.Value.UserName,
                Password = settings.Value.Password,
                VirtualHost = settings.Value.VirtualHost,
                Port = settings.Value.Port,
                DispatchConsumersAsync = true
            };

            if (settings.Value.SslEnabled)
            {
                _connectionFactory.Ssl = new SslOption
                {
                    Enabled = true,
                    ServerName = settings.Value.HostName
                };
            }
        }

        public bool IsConnected => _connection is { IsOpen: true } && !_disposed;

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                TryConnect();
            }

            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connection is available to create a channel");
            }

            return _connection!.CreateModel();
        }

        public bool TryConnect()
        {
            _logger.LogInformation("Trying to connect to RabbitMQ");

            lock (_syncRoot)
            {
                if (IsConnected)
                {
                    return true;
                }

                var retryAttempt = 0;
                Exception? lastException = null;

                while (retryAttempt < _retryCount)
                {
                    try
                    {
                        _connection = _connectionFactory.CreateConnection();
                        break;
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                        _logger.LogWarning(ex, "RabbitMQ connection attempt {Attempt} of {Count} failed", retryAttempt + 1, _retryCount);
                        
                        retryAttempt++;
                        Thread.Sleep(1000 * Math.Min(retryAttempt, 30)); // Progressive backoff
                    }
                }

                if (_connection == null)
                {
                    _logger.LogCritical(lastException, "All connection attempts to RabbitMQ failed");
                    return false;
                }

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;

                    _logger.LogInformation("RabbitMQ client connected to {HostName}:{Port}", 
                        _connectionFactory.HostName, _connectionFactory.Port); // This will work with ConnectionFactory
                    return true;
                }
                
                _logger.LogCritical("Cannot establish RabbitMQ connection");
                return false;
            }
        }

        private void OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("RabbitMQ connection is blocked. Reason: {Reason}", e.Reason);
            TryConnect();
        }

        private void OnCallbackException(object? sender, CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning(e.Exception, "RabbitMQ connection callback exception");
            TryConnect();
        }

        private void OnConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("RabbitMQ connection shutdown. Reply code: {ReplyCode}, Reply text: {ReplyText}",
                e.ReplyCode, e.ReplyText);
            
            TryConnect();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            try
            {
                _connection?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error disposing RabbitMQ connection");
            }
        }
    }
}