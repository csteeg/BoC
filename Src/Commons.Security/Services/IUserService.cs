using System;
using System.Collections.Generic;
using System.Linq;
using BoC.Security.Model;
using BoC.Services;

namespace BoC.Security.Services
{
    public enum PasswordFormat
    {
        Clear,
        Hashed,
        Encrypted
    }

    public interface IUserService : IModelService<User>
    {
        User FindUser(String login);
        User CreateUser(User user, String password);

        Boolean UserExists(String login);
        Boolean ChangePassword(String userName, String oldPassword, String newPassword);
        Boolean ChangePassword(User user, String oldPassword, String newPassword);
        void SetPassword(String login, String password);
        void SetPassword(User user, String password);
        String EncodePassword(String password);

        IQueryable<User> FindUsersByPartialLogin(String login);
        Int32 CountUsersByPartialLogin(String login);
        IQueryable<User> FindUsersByPartialEmail(String email);
        Int32 CountUsersByPartialEmail(String email);

        Int32 CountOnlineUsers(TimeSpan activitySpan);

        void UpdateActivity(String login);
        void UpdateActivity(User user);
        void UnlockUser(String login);
        void UnlockUser(User user);
        void LockUser(String login);
        void LockUser(User user);

        String FindLoginByEmail(String email);

        User Authenticate(String login, String password);

        #region Roles
        void AddUsersToRoles(ICollection<User> users, ICollection<Role> roles);
        void AddUsersToRoles(ICollection<String> logins, ICollection<String> roleNames);
        void RemoveUsersFromRoles(ICollection<String> logins, ICollection<String> roleNames);
        void RemoveUsersFromRoles(ICollection<User> users, ICollection<Role> roles);
        void CreateRole(String roleName);
        Role GetRole(Int64 id);
        Role FindRole(String roleName);
        Boolean RoleExists(String roleName);
        void DeleteRole(String roleName);
        void DeleteRole(Role role);
        void DeleteRole(String roleName, Boolean throwWhenUsed);
        void DeleteRole(Role role, Boolean throwWhenUsed);
        Role[] GetAllRoles();
        #endregion

        #region settings
        Boolean RequiresUniqueEmail { get; set; }
        Int32 MaxInvalidPasswordAttempts { get; set; }
        Int32 PasswordAttemptWindowMinutes { get; set; }
        PasswordFormat PasswordFormat { get; set; }
        Int32 MinRequiredNonAlphanumericCharacters { get; set; }
        Int32 MinRequiredPasswordLength { get; set; }
        string PasswordStrengthRegularExpression { get; set; }
        #endregion
    }
}