using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Text;
using Steeg.Framework.Security;
using Castle.Windsor;

namespace Steeg.Framework.Web.Security
{
    public class RoleProvider: System.Web.Security.RoleProvider
    {
        private IUserService userService;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            IWindsorContainer container = SteegWindsorContainer.Obtain();
            this.userService = container.Resolve<IUserService>();

            if (this.userService == null)
                throw new ProviderException("SteegFramework RoleProvider only works when used in combination with an IUserService. You should add this component to your container.");

            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = typeof(SteegFrameworkMemberShipProvider).Name;

            base.Initialize(name, config);

        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            this.userService.AddUsersToRoles(usernames, roleNames);
        }

        String applicationName;
        public override String ApplicationName
        {
            get
            {
                return applicationName;
            }
            set
            {
                applicationName = value;
            }
        }

        public override void CreateRole(String roleName)
        {
            this.userService.CreateRole(roleName);
        }

        public override bool DeleteRole(String roleName, bool throwOnPopulatedRole)
        {
            this.userService.DeleteRole(roleName, throwOnPopulatedRole);
            return true;
        }

        public override String[] FindUsersInRole(String roleName, String usernameToMatch)
        {
            Role role = this.userService.FindRole(roleName);
            if (role == null)
                throw new RoleNotFoundException(roleName);
            else
            {
                String[] users = new string[role.Users.Count];
                for (int i = 0; i < role.Users.Count; i++)
                    users[i] = role.Users[i].Login;
                return users;
            }
        }

        public override String[] GetAllRoles()
        {
            Role[] all = this.userService.GetAllRoles();
            String[] roles = new String[all.Length];
            for (int i = 0; i < all.Length; i++)
                roles[i] = all[i];
            return roles;
        }

        public override String[] GetRolesForUser(string username)
        {
            User user = this.userService.FindUser(username);
            if (user == null)
                throw new UserNotFoundException(username);
            return Role.ToString(user.Roles);

        }

        public override String[] GetUsersInRole(String roleName)
        {
            Role role = this.userService.FindRole(roleName);
            if (role == null)
                throw new RoleNotFoundException(roleName);
            else
                return User.ToString(role.Users);
        }

        public override bool IsUserInRole(String username, String roleName)
        {
            User user = this.userService.FindUser(username);
            if (user == null)
                throw new UserNotFoundException(username);
            else
                return user.IsInRole(roleName);
        }

        public override void RemoveUsersFromRoles(String[] usernames, String[] roleNames)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool RoleExists(String roleName)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
