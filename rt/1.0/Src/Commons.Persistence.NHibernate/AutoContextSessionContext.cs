using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web;
using BoC.InversionOfControl;
using NHibernate;
using NHibernate.Context;
using NHibernate.Engine;

namespace BoC.Persistence.NHibernate
{
    public class AutoContextSessionContext : MapBasedSessionContext
    {
        [ThreadStatic]
        protected static IDictionary map;
        private const string SessionFactoryMapKey = "NHibernate.Context.WebSessionContext.SessionFactoryMapKey";

        public AutoContextSessionContext(ISessionFactoryImplementor factory)
            : base(factory)
        {
        }

        protected override IDictionary GetMap()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Items[SessionFactoryMapKey] as IDictionary;
            }
            if (OperationContext.Current != null)
            {
                return WcfOperationState.Map;
            }
            return map;
        }

        protected override void SetMap(IDictionary value)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[SessionFactoryMapKey] = value;
            }
            else if (OperationContext.Current != null)
            {
                WcfOperationState.Map = value;
            }
            else 
            {
                map = value;
            }
        }

        private static WcfStateExtension WcfOperationState
        {
            get
            {
                var extension = OperationContext.Current.Extensions.Find<WcfStateExtension>();

                if (extension == null)
                {
                    extension = new WcfStateExtension();
                    OperationContext.Current.Extensions.Add(extension);
                }

                return extension;
            }
        }
    }

    public class WcfStateExtension : IExtension<OperationContext>
    {
        public IDictionary Map { get; set; }

        // we don't really need implementations for these methods in this case
        public void Attach(OperationContext owner) { }
        public void Detach(OperationContext owner) { }
    }
}
