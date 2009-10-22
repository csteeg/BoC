using System;
using System.Collections.Generic;
using System.Linq;

namespace BoC.Tasks
{
    public interface IBackgroundTask: IDisposable
    {
        bool IsRunning
        {
            get;
        }

        void Start();

        void Stop();
    }

}