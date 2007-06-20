using System;

namespace Steeg.Framework.Security
{
    public class UserServiceException : Exception
    {
        public UserServiceException() : base() { }
        public UserServiceException(String message) : base(message) { }
        public UserServiceException(String message, Exception innerException) : base(message, innerException) { }
        public UserServiceException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}