using System;
using System.Threading;


namespace EventBus.Composite.Presentation.Events
{
    /// <summary>
    /// Wraps the Application Dispatcher.
    /// </summary>
    public class DefaultDispatcher : IDispatcherFacade
    {
        private readonly SynchronizationContext _synchronizationContext;

        public DefaultDispatcher()
        {
            _synchronizationContext = SynchronizationContext.Current;
        }
        /// <summary>
        /// Forwards the BeginInvoke to the current application's <see cref="Dispatcher"/>.
        /// </summary>
        /// <param name="method">Method to be invoked.</param>
        /// <param name="arg">Arguments to pass to the invoked method.</param>
        public void BeginInvoke(Delegate method, object arg)
        {
            _synchronizationContext.Send((o) => method.DynamicInvoke(arg), null);
        }
    }
}