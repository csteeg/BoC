using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using NHibernate;

namespace BoC.Persistence.NHibernate.DataContext
{
    public class DataContextSessionManager: ISessionManager
    {
        public ISession Session
        {
            get
            {
                if (NHibernateDataContext.OuterDataContext == null)
                {
                    throw new DataContextException("You are using DataContextSessionManager but are accessing a session outside a unit of work");
                }
                return ((NHibernateDataContext)NHibernateDataContext.OuterDataContext).Session;
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