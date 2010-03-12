using System.Threading;
using BoC.EventAggregator;
using BoC.Helpers;

namespace BoC.Tasks
{
    using System;
    using System.Diagnostics;

    public abstract class BaseThreadedBackgroundTask : BaseBackgroundTask
    {
        private volatile bool stopping = false;
        public bool IsStopping
        {
            get { return stopping; }
            protected set
            {
                stopping = value;
            }
        }

        private Thread workThread;

        protected BaseThreadedBackgroundTask(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        /// <summary>
        /// This method is started in a background thread
        /// </summary>
        public abstract void DoWork();

        protected override void OnStart()
        {
            //allows initialization stuff
        }

        protected override void OnStop()
        {
            //allows cleanup stuff
        }

        override public void Start()
        {
            OnStart();
            workThread = new Thread(DoWork);
            workThread.Start();
            if (stopping) //if we have to stop immediately...
                workThread.Join();
            IsRunning = true;
        }

        override public void Stop()
        {
            IsStopping = true;
            OnStop();
            if (workThread != null)
                workThread.Join();
            IsRunning = false;
        }

    }
}