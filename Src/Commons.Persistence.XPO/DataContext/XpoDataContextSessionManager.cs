using System;
using System.Runtime.Serialization;
using DevExpress.Xpo;

namespace BoC.Persistence.Xpo.DataContext
{
    public class XpoDataContextSessionManager: ISessionManager
    {
        public Session Session
        {
            get
            {
                if (XpoDataContext.OuterDataContext == null)
                {
                    throw new DataContextException("You are using DataContextSessionManager but are accessing a session outside a unit of work");
                }
                return ((XpoDataContext)XpoDataContext.OuterDataContext).Session;
            }
        }


    }

    public class DataContextException : Exception
    {
        public DataContextException() { }
        public DataContextException(string message) : base(message) { }
        public DataContextException(string message, Exception innerException) : base(message, innerException) { }
        protected DataContextException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}