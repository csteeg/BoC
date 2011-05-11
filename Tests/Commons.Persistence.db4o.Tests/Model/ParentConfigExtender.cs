using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoC.InversionOfControl;
using BoC.Persistence.db4o;
using Db4objects.Db4o.Config;

namespace Commons.Persistence.db4o.Tests.Model
{
    public class ParentConfigExtender : IConfigurationExtender, IContainerInitializer
    {
        private readonly IDependencyResolver dependencyResolver;

        public ParentConfigExtender(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Configure(IEmbeddedConfiguration configuration)
        {
            configuration.Common.ObjectClass(typeof(Parent)).CascadeOnDelete(true);
        }

        public void Execute()
        {
            dependencyResolver.RegisterInstance<IConfigurationExtender>(this);
        }
    }
}
