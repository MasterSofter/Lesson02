using System;

namespace EventBus.Interfaces
{
    public interface ISubscriptionToken : IEquatable<ISubscriptionToken>
    {
        Guid Token { get; set; }
    }
}
