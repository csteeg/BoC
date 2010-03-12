using System;

namespace BoC.Security
{
    public class RoleNotFoundException : UserServiceException
    {
        public RoleNotFoundException() : base("RoleName", "The role could not be found") { }
        public RoleNotFoundException(String role) : base("RoleName", string.Format("The role with name {0} could not be found", role)) { }
    }
}