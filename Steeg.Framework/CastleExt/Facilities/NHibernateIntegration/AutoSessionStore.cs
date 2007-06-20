namespace Steeg.Framework.CastleExt.Facilities.NHibernateIntegration
{
    using System.Web;
    using System.Collections;
    using System.Runtime.Remoting.Messaging;
    using Castle.MicroKernel.Facilities;

    /// <summary>
    /// Provides an implementation of <see cref="ISessionStore"/>
    /// which relies on <c>HttpContext</c>. Suitable for web projects.
    /// </summary>
    public class AutoSessionStore : Castle.Facilities.NHibernateIntegration.Internal.AbstractDictStackSessionStore
    {
        protected override IDictionary GetDictionary()
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.Items[SlotKey] as IDictionary;
            else
                return CallContext.GetData(SlotKey) as IDictionary;
        }

        protected override void StoreDictionary(IDictionary dictionary)
        {
            if (HttpContext.Current != null)
                HttpContext.Current.Items[SlotKey] = dictionary;
            else
                CallContext.SetData(SlotKey, dictionary);
        }
    }
}
