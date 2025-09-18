using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Messaging.Abstractions;
using Shared.Messaging.Configuration;
using Shared.Messaging.Infrastructure;

namespace Shared.Messaging
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMQSettings>(configuration.GetSection(RabbitMQSettings.SectionName));
            services.AddSingleton<RabbitMQConnection>();
            services.AddSingleton<IEventBus, EventBusRabbitMQ>();

            return services;
        }
    }
}