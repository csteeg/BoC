using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using BoC.InversionOfControl;

namespace BoC.ServiceModel
{
    public class IoCInstanceProvider: IInstanceProvider
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly Type type;

        public IoCInstanceProvider(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public IoCInstanceProvider(Type type)
        {
            this.type = type;
            //test if we can create instances for this type
            dependencyResolver.Resolve(type);
        }

        #region IInstanceProvider Members

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return dependencyResolver.Resolve(type);
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            if (instance is IDisposable)
                ((IDisposable)instance).Dispose();
        }
        #endregion
    }
}
