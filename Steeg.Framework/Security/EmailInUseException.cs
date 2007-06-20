using System;

namespace Steeg.Framework.Security
{
    public class EmailInUseException : UserServiceException
    {
        public EmailInUseException() : base() { }
        public EmailInUseException(String email) : base(string.Format("The email address {0} is already registered", email)) { }
        public EmailInUseException(String email, Exception innerException) : base(string.Format("The email address {0} is already registered", email), innerException) { }
        public EmailInUseException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}