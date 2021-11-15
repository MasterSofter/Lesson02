using System;
using System.Collections.Generic;
using EventBus.Interfaces;
 
namespace EventBus.Composite.Events
{
    /// <summary>
    /// Implements <see cref="IEventBus"/>.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly List<IEventBase> _events = new List<IEventBase>();

        /// <summary>
        /// Gets the single instance of the event managed by this EventAggregator. Multiple calls to this method with the same <typeparamref name="TEventType"/> returns the same event instance.
        /// </summary>
        /// <typeparam name="TEventType">The type of event to get. This must inherit from <see cref="EventBase"/>.</typeparam>
        /// <returns>A singleton instance of an event object of type <typeparamref name="TEventType"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public TEventType GetEvent<TEventType>() where TEventType : IEventBase
        {
            IEventBase eventInstance = _events.Find(o => o.GetType() == typeof(TEventType));
            if (eventInstance == null)
            {
                eventInstance = Activator.CreateInstance<TEventType>();
                _events.Add(eventInstance);
            }
            return (TEventType)eventInstance;
        }
    }
}
