using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Messaging.Messages;

namespace Shared.Messaging.Abstractions
{
    public abstract class BaseConsumer<TMessage> : BackgroundService where TMessage : class
    {
        protected readonly IEventBus EventBus;
        protected readonly IServiceProvider ServiceProvider;
        protected readonly ILogger Logger;
        protected readonly string RoutingKey;

        protected BaseConsumer(
            IEventBus eventBus,
            IServiceProvider serviceProvider,
            ILogger logger,
            string routingKey)
        {
            EventBus = eventBus;
            ServiceProvider = serviceProvider;
            Logger = logger;
            RoutingKey = routingKey;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("Starting consumer for {MessageType} with routing key {RoutingKey}",
                typeof(TMessage).Name, RoutingKey);

            EventBus.Subscribe<TMessage>(RoutingKey, HandleMessage);

            return Task.CompletedTask;
        }

        protected abstract Task HandleMessage(TMessage message);

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            EventBus.Unsubscribe<TMessage>(RoutingKey);
            Logger.LogInformation("Stopped consumer for {MessageType} with routing key {RoutingKey}",
                typeof(TMessage).Name, RoutingKey);

            return base.StopAsync(cancellationToken);
        }
    }
}