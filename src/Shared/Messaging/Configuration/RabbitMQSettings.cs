namespace Shared.Messaging.Configuration
{
    public class RabbitMQSettings
    {
        public const string SectionName = "RabbitMQ";
        
        public string HostName { get; set; } = "localhost";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public int Port { get; set; } = 5672;
        public string ExchangeName { get; set; } = "microservices_exchange";
        public bool SslEnabled { get; set; } = false;
        public int ConnectionRetryCount { get; set; } = 5;
    }
}