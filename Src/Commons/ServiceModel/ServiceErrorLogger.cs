using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using BoC.Logging;

namespace BoC.ServiceModel
{
    public class ServiceErrorLogger : IErrorHandler
    {
        private readonly ILogger logger;

        public ServiceErrorLogger(ILogger logger)
        {
            this.logger = logger;
        }

        // Provide a fault. The Message fault parameter can be replaced, or set to
        // null to suppress reporting a fault.
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
        }

        // HandleError. Log an error, then allow the error to be handled as usual.
        // Return true if the error is considered as already handled
        public bool HandleError(Exception error)
        {
            logger.Error("Service exception occurred", error);
            return false;
        }

    }
}
