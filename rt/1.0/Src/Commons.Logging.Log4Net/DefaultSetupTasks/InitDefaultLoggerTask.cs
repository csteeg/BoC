using BoC.InversionOfControl;
using BoC.InversionOfControl.Configuration;

namespace BoC.Logging.Log4Net.DefaultSetupTasks
{
    public class InitDefaultLoggerTask : IContainerInitializer
    {
        public void Execute()
        {
            if (!IoC.IsRegistered<ILogger>())
            {
                log4net.Config.XmlConfigurator.Configure();
                IoC.RegisterType<ILogger, Log4netLogger>();
            }
        }
    }
}