using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using NHibernate;

namespace BoC.Persistence.NHibernate.UnitOfWork
{
    public class UnitOfWorkSessionManager: ISessionManager
    {
        public ISession Session
        {
            get
            {
                if (NHibernateUnitOfWork.OuterUnitOfWork == null)
                {
                    throw new UnitOfWorkException("You are using UnitOfWorkSessionManager but are accessing a session outside a unit of work");
                }
                return ((NHibernateUnitOfWork)NHibernateUnitOfWork.OuterUnitOfWork).Session;
            }
        }


    }

    public class UnitOfWorkException : Exception
    {
        public UnitOfWorkException() { }
        public UnitOfWorkException(string message) : base(message) { }
        public UnitOfWorkException(string message, Exception innerException) : base(message, innerException) { }
        protected UnitOfWorkException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}