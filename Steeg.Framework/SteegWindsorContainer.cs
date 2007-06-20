using Castle.Windsor;
using Castle.Core.Logging;
namespace Steeg.Framework
{
    public class SteegWindsorContainer: WindsorContainer
    {
        public SteegWindsorContainer()
            : base( new Castle.Windsor.Configuration.Interpreters.XmlInterpreter("config/steeg-framework.config") )
        {
            
        }

        protected override void RunInstaller()
        {
            base.RunInstaller();
            if (!this.Kernel.HasComponent(typeof(ILogger)))
            {
                this.Kernel.AddComponent("steeg.framework.logger", typeof(ILogger), typeof(NullLogger)); ;
            }
        }

        public static SteegWindsorContainer Obtain()
        {
            return (SteegWindsorContainer)Web.SteegApplicationModule.GetContainer();
        }

    }
}
