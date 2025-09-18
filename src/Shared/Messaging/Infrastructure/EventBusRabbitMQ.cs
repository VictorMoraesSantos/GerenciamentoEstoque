using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Messaging.Abstractions;
using Shared.Messaging.Configuration;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messaging.Infrastructure
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private readonly RabbitMQConnection _connection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly string _exchangeName;
        private readonly ConcurrentDictionary<string, IModel> _consumerChannels;
        private readonly ConcurrentDictionary<string, List<Delegate>> _handlers;

        public EventBusRabbitMQ(
            RabbitMQConnection connection,
            IOptions<RabbitMQSettings> settings,
            ILogger<EventBusRabbitMQ> logger)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _exchangeName = settings.Value.ExchangeName;
            _consumerChannels = new ConcurrentDictionary<string, IModel>();
            _handlers = new ConcurrentDictionary<string, List<Delegate>>();

            EnsureExchangeExists();
        }

        private void EnsureExchangeExists()
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using var channel = _connection.CreateModel();
            channel.ExchangeDeclare(
                exchange: _exchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);
        }

        public void Publish<T>(string routingKey, T message) where T : class
        {
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            using var channel = _connection.CreateModel();
            
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2; // Persistent
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.ContentType = "application/json";
            properties.Type = typeof(T).Name;

            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            _logger.LogInformation("Publishing message to {RoutingKey}: {Message}", routingKey, messageJson);

            channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: properties,
                body: body);
        }

        public void Subscribe<T>(string routingKey, Func<T, Task> handler) where T : class
        {
            var queueName = $"{typeof(T).Name}_{routingKey}";
            
            if (!_connection.IsConnected)
            {
                _connection.TryConnect();
            }

            if (!_consumerChannels.TryGetValue(queueName, out var channel))
            {
                channel = _connection.CreateModel();
                _consumerChannels[queueName] = channel;
            }

            channel.ExchangeDeclare(
                exchange: _exchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            channel.QueueBind(
                queue: queueName,
                exchange: _exchangeName,
                routingKey: routingKey);

            if (!_handlers.TryGetValue(queueName, out var handlers))
            {
                handlers = new List<Delegate>();
                _handlers[queueName] = handlers;
                
                var consumer = new AsyncEventingBasicConsumer(channel);
                
                consumer.Received += async (sender, eventArgs) =>
                {
                    var message = Encoding.UTF8.GetString(eventArgs.Body.Span);
                    _logger.LogInformation("Message received from {RoutingKey}: {Message}", routingKey, message);

                    try
                    {
                        await ProcessEvent(queueName, message);
                        channel.BasicAck(eventArgs.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message from {RoutingKey}", routingKey);
                        channel.BasicNack(eventArgs.DeliveryTag, false, true); // Requeue
                    }
                };

                channel.BasicConsume(
                    queue: queueName,
                    autoAck: false,
                    consumer: consumer);
                    
                _logger.LogInformation("Subscribed to {RoutingKey} with queue {QueueName}", routingKey, queueName);
            }

            handlers.Add(handler);
        }

        public void Unsubscribe<T>(string routingKey) where T : class
        {
            var queueName = $"{typeof(T).Name}_{routingKey}";
            
            if (_consumerChannels.TryRemove(queueName, out var channel))
            {
                channel.Close();
                _logger.LogInformation("Unsubscribed from {RoutingKey} with queue {QueueName}", routingKey, queueName);
            }
            
            _handlers.TryRemove(queueName, out _);
        }

        private async Task ProcessEvent(string queueName, string message)
        {
            if (_handlers.TryGetValue(queueName, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    var eventType = handler.GetType().GenericTypeArguments[0];
                    var eventData = JsonConvert.DeserializeObject(message, eventType);
                    
                    if (eventData == null) continue;

                    var concreteType = typeof(Func<,>).MakeGenericType(eventType, typeof(Task));
                    await (Task)handler.DynamicInvoke(eventData)!;
                }
            }
        }

        public void Dispose()
        {
            foreach (var channel in _consumerChannels.Values)
            {
                channel.Close();
                channel.Dispose();
            }
        }
    }
}