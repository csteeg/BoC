using System;
using System.ServiceModel;
using System.Web;

namespace BoC.UnitOfWork
{
    public abstract class BaseThreadSafeSingleUnitOfWork: IUnitOfWork
    {
        [ThreadStatic]
        private static IUnitOfWork _outerUnitOfWorkThreadstatic;   
        [ThreadStatic]
        private static IUnitOfWork _currentUnitOfWorkThreadstatic;

        private IUnitOfWork _previous;
        private const string Outerunitofworkmapkey = "IUnitOfWork.outerUnitOfWork";
        private const string Currentunitofworkmapkey = "IUnitOfWork.currentUnitOfWork";

        protected BaseThreadSafeSingleUnitOfWork()
        {
            if (OuterUnitOfWork == null)
            {
                OuterUnitOfWork = this;
            }
            _previous = CurrentUnitOfWork;
            CurrentUnitOfWork = this;
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
                    return HttpContext.Current.Items[Outerunitofworkmapkey] as IUnitOfWork;
                }
                if (OperationContext.Current != null)
                {
                    return WcfOperationState.OuterUnitOfWork as IUnitOfWork;
                }
                return _outerUnitOfWorkThreadstatic;
            }
            private set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[Outerunitofworkmapkey] = value;
                }
                else if (OperationContext.Current != null)
                {
                    WcfOperationState.OuterUnitOfWork = value;
                }
                else
                {
                    _outerUnitOfWorkThreadstatic = value;
                }

            }
        }

        /// <summary>
        /// Managed way to keep the outer unitofwork thread safe.
        /// Threadstatic is not safe in asp.net environment :(
        /// see: http://www.hanselman.com/blog/ATaleOfTwoTechniquesTheThreadStaticAttributeAndSystemWebHttpContextCurrentItems.aspx
        /// </summary>
        public static IUnitOfWork CurrentUnitOfWork
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Items[Currentunitofworkmapkey] as IUnitOfWork;
                }
                if (OperationContext.Current != null)
                {
                    return WcfOperationState.CurrentUnitOfWork as IUnitOfWork;
                }
                return _currentUnitOfWorkThreadstatic;
            }
            private set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[Currentunitofworkmapkey] = value;
                }
                else if (OperationContext.Current != null)
                {
                    WcfOperationState.CurrentUnitOfWork = value;
                }
                else
                {
                    _currentUnitOfWorkThreadstatic = value;
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
                CleanUpOuterUnitOfWork();
                OuterUnitOfWork = null;
            }
            CurrentUnitOfWork = _previous;
        }

        protected abstract void CleanUpOuterUnitOfWork();

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
        public IUnitOfWork CurrentUnitOfWork { get; set; }

        // we don't really need implementations for these methods in this case
        public void Attach(OperationContext owner) { }
        public void Detach(OperationContext owner) { }
    }
}
