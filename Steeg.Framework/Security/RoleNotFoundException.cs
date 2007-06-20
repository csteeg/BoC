using System;

namespace Steeg.Framework.Security
{
    public class RoleNotFoundException : UserServiceException
    {
        public RoleNotFoundException() : base() { }
        public RoleNotFoundException(String role) : base(string.Format("The role with name {0} could not be found", role)) { }
        public RoleNotFoundException(String role, Exception innerException) : base(string.Format("The role with name {0} could not be found", role), innerException) { }
        public RoleNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}