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
            workThread = new Thread(DoWork);
            workThread.Start();
            if (stopping) //if we have to stop immediately...
                Stop();
        }

        protected override void OnStop()
        {
            IsStopping = true;
            if (workThread != null)
                workThread.Join();
        }
    }
}