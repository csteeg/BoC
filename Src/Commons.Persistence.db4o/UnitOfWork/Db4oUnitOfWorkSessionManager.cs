using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Db4objects.Db4o;

namespace Commons.Persistence.db4o.UnitOfWork
{
    public class Db4oUnitOfWorkSessionManager : ISessionManager
    {
        public IObjectContainer Session
        {
            get
            {
                if (Db4oUnitOfWork.OuterUnitOfWork == null)
                {
                    throw new UnitOfWorkException("You are using UnitOfWorkSessionManager but are accessing a session outside a unit of work");
                }
                return ((Db4oUnitOfWork)Db4oUnitOfWork.OuterUnitOfWork).Session;
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
