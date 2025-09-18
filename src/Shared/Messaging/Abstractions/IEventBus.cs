namespace Shared.Messaging.Abstractions
{
    public interface IEventBus
    {
        void Publish<T>(string routingKey, T message) where T : class;
        
        void Subscribe<T>(string routingKey, Func<T, Task> handler) where T : class;
        
        void Unsubscribe<T>(string routingKey) where T : class;
    }
}