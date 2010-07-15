using System.Threading;
using BoC.EventAggregator;
using BoC.Helpers;

namespace BoC.Tasks
{
    using System;
    using System.Diagnostics;

    public abstract class BaseBackgroundTask : IBackgroundTask
    {
        private readonly IEventAggregator _eventAggregator;
        private volatile bool running = false;

        protected BaseBackgroundTask(IEventAggregator eventAggregator)
        {
            Check.Argument.IsNotNull(eventAggregator, "eventAggregator");

            _eventAggregator = eventAggregator;
        }

        public bool IsRunning
        {
            get { return running; }
            protected set { running = value; }
        }

        protected IEventAggregator EventAggregator
        {
            get
            {
                return _eventAggregator;
            }
        }

        public void Start()
        {
            IsRunning = true;
            OnStart();
        }

        public void Stop()
        {
            OnStop();
            IsRunning = false;
        }

        protected abstract void OnStart();

        protected abstract void OnStop();

        protected SubscriptionToken Subscribe<TEvent,TEventArgs>(Action<TEventArgs> action) where TEvent : BaseEvent<TEventArgs>, new() where TEventArgs : class
        {
            return EventAggregator.GetEvent<TEvent>().Subscribe(action, true);
        }

        protected void Unsubscribe<TEvent>(SubscriptionToken token) where TEvent : BaseEvent, new()
        {
            EventAggregator.GetEvent<TEvent>().Unsubscribe(token);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}