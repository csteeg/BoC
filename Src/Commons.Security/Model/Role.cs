using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BoC.Persistence;

namespace BoC.Security.Model
{
    public class Role: BaseEntity<long>
    {
        public Role() { }
        public Role(String roleName) 
        {
            this.RoleName = roleName;
        }

        [Required]
        virtual public string RoleName { get; set; }
        
        ICollection<User> users = new HashSet<User>();
        virtual public ICollection<User> Users
        {
            get { return this.users; }
            protected set { this.users = value; }
        }

        public override String ToString()
        {
            return this.RoleName;
        }

    }
}