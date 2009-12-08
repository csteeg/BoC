using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BoC.Persistence.Exceptions
{
    public abstract  class BasePersistenceException: Exception
    {
        protected BasePersistenceException() {}
        protected BasePersistenceException(string message) : base(message) {}
        protected BasePersistenceException(string message, Exception innerException) : base(message, innerException) {}
        protected BasePersistenceException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
    public class ObjectNotUniqueException: BasePersistenceException
    {
        public ObjectNotUniqueException() {}
        public ObjectNotUniqueException(string message) : base(message) {}
        public ObjectNotUniqueException(string message, Exception innerException) : base(message, innerException) {}
        public ObjectNotUniqueException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
    public class ObjectNotFoundException: BasePersistenceException
    {
        public ObjectNotFoundException() {}
        public ObjectNotFoundException(string message) : base(message) {}
        public ObjectNotFoundException(string message, Exception innerException) : base(message, innerException) {}
        public ObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}
