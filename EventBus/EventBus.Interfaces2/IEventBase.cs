namespace EventBus.Interfaces
{
    public interface IEventBase
    {
        void Unsubscribe(ISubscriptionToken token);
        bool Contains(ISubscriptionToken token);
    }
}
