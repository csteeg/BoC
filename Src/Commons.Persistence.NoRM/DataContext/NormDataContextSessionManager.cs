using System;
using System.Runtime.Serialization;
using BoC.Persistence.Norm;
using Norm;

namespace BoC.Persistence.Norm.DataContext
{
    public class NormDataContextSessionManager: ISessionManager
    {
        public IMongo Session
        {
            get
            {
                if (NormDataContext.OuterDataContext == null)
                {
                    throw new DataContextException("You are using DataContextSessionManager but are accessing a session outside a unit of work");
                }
                return ((NormDataContext)NormDataContext.OuterDataContext).Session;
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