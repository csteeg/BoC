using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;

namespace BoC.UnitOfWork
{
    public abstract class BaseThreadSafeSingleUnitOfWork: IUnitOfWork
    {
        [ThreadStatic]
        private static IUnitOfWork outerUnitOfWork_threadstatic = null;

        private static readonly string outerunitofworkmapkey = "NHibernateUnitOfWork.outerUnitOfWork";

        protected BaseThreadSafeSingleUnitOfWork()
        {
            if (OuterUnitOfWork == null)
            {
                OuterUnitOfWork = this;
            }
        }

        /// <summary>
        /// Managed way to keep the outer unitofwork thread safe.
        /// Threadstatic is not safe in asp.net environment :(
        /// see: http://www.hanselman.com/blog/ATaleOfTwoTechniquesTheThreadStaticAttributeAndSystemWebHttpContextCurrentItems.aspx
        /// </summary>
        public static IUnitOfWork OuterUnitOfWork
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Items[outerunitofworkmapkey] as IUnitOfWork;
                }
                if (OperationContext.Current != null)
                {
                    return WcfOperationState.OuterUnitOfWork as IUnitOfWork;
                }
                return outerUnitOfWork_threadstatic;
            }
            private set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[outerunitofworkmapkey] = value;
                }
                else if (OperationContext.Current != null)
                {
                    WcfOperationState.OuterUnitOfWork = value;
                }
                else
                {
                    outerUnitOfWork_threadstatic = value;
                }

            }
        }

        ~BaseThreadSafeSingleUnitOfWork()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (OuterUnitOfWork == this)
            {
                CleanUp();
                OuterUnitOfWork = null;
            }
        }

        protected abstract void CleanUp();

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
        public IUnitOfWork OuterUnitOfWork { get; set; }

        // we don't really need implementations for these methods in this case
        public void Attach(OperationContext owner) { }
        public void Detach(OperationContext owner) { }
    }
}
