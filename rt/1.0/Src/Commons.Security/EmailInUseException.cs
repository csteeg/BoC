using System;
using BoC.Security;

namespace BoC.Security
{
    public class EmailInUseException : UserServiceException
    {
        public EmailInUseException(String email) : base("Email", string.Format("The email address {0} is already registered", email)) { }
    }
}