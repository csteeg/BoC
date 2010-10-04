using System;
using System.Runtime.Serialization;
using BoC.Persistence.Norm;
using Norm;

namespace BoC.Persistence.Norm.UnitOfWork
{
    public class NormUnitOfWorkSessionManager: ISessionManager
    {
        public IMongo Session
        {
            get
            {
                if (NormUnitOfWork.OuterUnitOfWork == null)
                {
                    throw new UnitOfWorkException("You are using UnitOfWorkSessionManager but are accessing a session outside a unit of work");
                }
                return ((NormUnitOfWork)NormUnitOfWork.OuterUnitOfWork).Session;
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