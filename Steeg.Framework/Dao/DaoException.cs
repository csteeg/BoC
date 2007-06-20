using System;

namespace Steeg.Framework.Dao
{
    public class DaoException : Exception
    {
        public DaoException() : base() { }
        public DaoException(String message) : base(message) { }
        public DaoException(String message, Exception innerException) : base(message, innerException) { }
        public DaoException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }
}