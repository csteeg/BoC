using System;

namespace Steeg.Framework.Security
{
    public class LoginInUseException : UserServiceException
    {
        public LoginInUseException () : base() { }
        public LoginInUseException (String login) : this(login, null) { }
        public LoginInUseException(String login, Exception innerException) : base(string.Format("A user with login {0} already exists", login), innerException) { }
        public LoginInUseException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}