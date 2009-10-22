using System;

namespace BoC.Security
{
    public class LoginInUseException : UserServiceException
    {
        public LoginInUseException(String login) : base("Login", string.Format("A user with login {0} already exists", login)) { }
    }
}