using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Permissions;
using Steeg.Framework.Security;

namespace Steeg.Framework.Security
{
    public enum PasswordFormat
    {
        Clear,
        Hashed,
        Encrypted
    }

    public interface IUserService
    {
        User GetUser(Int64 userId);
        User FindUser(String login);

        Boolean UserExists(String login);
        Boolean ChangePassword(String userName, String oldPassword, String newPassword);
        Boolean ChangePassword(User user, String oldPassword, String newPassword);
        void ChangePasswordQuestionAndAnswer(User user, String newPwdQuestion, String newPwdAnswer);
        void ChangePasswordQuestionAndAnswer(String username, String newPwdQuestion, String newPwdAnswer);
        void SetPassword(String login, String password);
        void SetPassword(User user, String password);
        String ResetPassword(String login, String password);
        String ResetPassword(User user, String password);
        String GetPassword(String username, String answer);
        String GetPassword(User user, String answer);

        User CreateUser(String login,String password,String email,String name,String passwordQuestion,String passwordAnswer,Boolean isApproved);
        void DeleteUser(String login);
        void DeleteUser(User user);
        void UpdateUser(String login, String email, String name, Boolean isApproved);
        void UpdateUser(User user, String email, String name, Boolean isApproved);

        Int32 CountUsers();

        User[] GetAllUsers();
        User[] GetAllUsers(Int32 startRow, Int32 maxRows);
        User[] FindUsersByPartialLogin(String login, Int32 startRow, Int32 maxRows);
        Int32 CountUsersByPartialLogin(String login);
        User[] FindUsersByPartialEmail(String email, Int32 startRow, Int32 maxRows);
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
        Boolean EnablePasswordReset { get;}
        Boolean EnablePasswordRetrieval { get;}
        Boolean RequiresQuestionAndAnswer { get;}
        Boolean RequiresUniqueEmail { get;}
        Int32 MaxInvalidPasswordAttempts{get;}
        Int32 PasswordAttemptWindowMinutes { get;}
        PasswordFormat PasswordFormat{get;}
        Int32 MinRequiredNonAlphanumericCharacters { get;}
        Int32 MinRequiredPasswordLength { get;}
        string PasswordStrengthRegularExpression{get;}
        #endregion
    }
}
