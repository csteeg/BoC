using System;
using System.Runtime.Serialization;
using DevExpress.Xpo;

namespace BoC.Persistence.Xpo.UnitOfWork
{
    public class XpoUnitOfWorkSessionManager: ISessionManager
    {
        public Session Session
        {
            get
            {
                if (XpoUnitOfWork.OuterUnitOfWork == null)
                {
                    throw new UnitOfWorkException("You are using UnitOfWorkSessionManager but are accessing a session outside a unit of work");
                }
                return ((XpoUnitOfWork)XpoUnitOfWork.OuterUnitOfWork).Session;
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