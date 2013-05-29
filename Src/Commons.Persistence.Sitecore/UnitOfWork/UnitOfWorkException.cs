using System;
using System.Runtime.Serialization;

namespace BoC.Persistence.SitecoreGlass.UnitOfWork
{
    public class UnitOfWorkException : Exception
    {
        public UnitOfWorkException() { }
        public UnitOfWorkException(string message) : base(message) { }
        public UnitOfWorkException(string message, Exception innerException) : base(message, innerException) { }
        protected UnitOfWorkException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}