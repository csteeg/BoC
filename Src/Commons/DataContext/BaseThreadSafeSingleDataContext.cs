using System;
using System.ServiceModel;
using System.Web;

namespace BoC.DataContext
{
    public abstract class BaseThreadSafeSingleDataContext: IDataContext
    {
        [ThreadStatic]
        private static IDataContext _outerDataContextThreadstatic;   
        [ThreadStatic]
        private static IDataContext _currentDataContextThreadstatic;

        private IDataContext _previous;
        private const string OuterDataContextmapkey = "IDataContext.outerDataContext";
        private const string CurrentDataContextmapkey = "IDataContext.currentDataContext";

        protected BaseThreadSafeSingleDataContext()
        {
            if (OuterDataContext == null)
            {
                OuterDataContext = this;
            }
            _previous = CurrentDataContext;
            CurrentDataContext = this;
        }

        /// <summary>
        /// Managed way to keep the outer DataContext thread safe.
        /// Threadstatic is not safe in asp.net environment :(
        /// see: http://www.hanselman.com/blog/ATaleOfTwoTechniquesTheThreadStaticAttributeAndSystemWebHttpContextCurrentItems.aspx
        /// </summary>
        public static IDataContext OuterDataContext
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Items[OuterDataContextmapkey] as IDataContext;
                }
                if (OperationContext.Current != null)
                {
                    return WcfOperationState.OuterDataContext as IDataContext;
                }
                return _outerDataContextThreadstatic;
            }
            private set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[OuterDataContextmapkey] = value;
                }
                else if (OperationContext.Current != null)
                {
                    WcfOperationState.OuterDataContext = value;
                }
                else
                {
                    _outerDataContextThreadstatic = value;
                }

            }
        }

        /// <summary>
        /// Managed way to keep the outer DataContext thread safe.
        /// Threadstatic is not safe in asp.net environment :(
        /// see: http://www.hanselman.com/blog/ATaleOfTwoTechniquesTheThreadStaticAttributeAndSystemWebHttpContextCurrentItems.aspx
        /// </summary>
        public static IDataContext CurrentDataContext
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Items[CurrentDataContextmapkey] as IDataContext;
                }
                if (OperationContext.Current != null)
                {
                    return WcfOperationState.CurrentDataContext as IDataContext;
                }
                return _currentDataContextThreadstatic;
            }
            private set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[CurrentDataContextmapkey] = value;
                }
                else if (OperationContext.Current != null)
                {
                    WcfOperationState.CurrentDataContext = value;
                }
                else
                {
                    _currentDataContextThreadstatic = value;
                }

            }
        }

        ~BaseThreadSafeSingleDataContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (OuterDataContext == this)
            {
                CleanUpOuterDataContext();
                OuterDataContext = null;
            }
            CurrentDataContext = _previous;
        }

        protected abstract void CleanUpOuterDataContext();

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
        public IDataContext OuterDataContext { get; set; }
        public IDataContext CurrentDataContext { get; set; }

        // we don't really need implementations for these methods in this case
        public void Attach(OperationContext owner) { }
        public void Detach(OperationContext owner) { }
    }
}
