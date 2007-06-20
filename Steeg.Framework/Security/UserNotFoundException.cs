using System;

namespace Steeg.Framework.Security
{
    public class UserNotFoundException : UserServiceException
    {
        public UserNotFoundException() : base() { }
        public UserNotFoundException(String login) : base(string.Format("The user with login {0} could not be found", login)) { }
        public UserNotFoundException(String login, Exception innerException) : base(string.Format("The user with login {0} could not be found", login), innerException) { }
        public UserNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}