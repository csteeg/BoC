using System;

namespace BoC.Security
{
    public class UserNotFoundException : UserServiceException
    {
        public UserNotFoundException() : base("Login", "The user could not be found") { }
        public UserNotFoundException(String login) : base("Login", string.Format("The user with login {0} could not be found", login)) { }
    }
}