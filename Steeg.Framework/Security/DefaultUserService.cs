using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Configuration;

using Steeg.Framework.Security;
using Steeg.Framework.Dao;

using System.Security.Permissions;
using System.Security.Cryptography;
using System.Transactions;

namespace Steeg.Framework.Security
{
    public class DefaultUserService : IUserService
    {
        const Int32 SALT_BYTES = 16;
        private Int32 newPasswordLength = 8;

        private IUserDao userDao;
        private IRoleDao roleDao;
        private Boolean enablePasswordReset = true;
        private Boolean enablePasswordRetrieval = false;
        private Boolean requiresQuestionAndAnswer = false;
        private Boolean requiresUniqueEmail = true;
        private Int32 maxInvalidPasswordAttempts = 10;
        private Int32 passwordAttemptWindowMinutes = 10;
        private PasswordFormat passwordFormat = PasswordFormat.Hashed;
        private Int32 minRequiredNonAlphanumericCharacters = 0;
        private Int32 minRequiredPasswordLength = 5;
        private String passwordStrengthRegularExpression;

        public DefaultUserService(IUserDao dao, IRoleDao roleDao)
        {
            this.userDao = dao;
        }

        #region IUserService Settings
        public Boolean EnablePasswordReset
        {
            get { return enablePasswordReset; }
        }

        public Boolean EnablePasswordRetrieval
        {
            get { return enablePasswordRetrieval; }
        }

        public Boolean RequiresQuestionAndAnswer
        {
            get { return requiresQuestionAndAnswer; }
        }

        public Boolean RequiresUniqueEmail
        {
            get { return requiresUniqueEmail; }
        }
        
        public Int32 MaxInvalidPasswordAttempts
        {
            get { return maxInvalidPasswordAttempts; }
        }

        public Int32 PasswordAttemptWindowMinutes
        {
            get { return passwordAttemptWindowMinutes; }
        }

        public PasswordFormat PasswordFormat
        {
            get { return passwordFormat; }
        }

        public Int32 MinRequiredNonAlphanumericCharacters
        {
            get { return minRequiredNonAlphanumericCharacters; }
        }

        public Int32 MinRequiredPasswordLength
        {
            get { return minRequiredPasswordLength; }
        }

        public String PasswordStrengthRegularExpression
        {
            get { return passwordStrengthRegularExpression; }
        }

        #endregion

        #region IUserService Members

        virtual public User GetUser(Int64 userId)
        {
            return this.userDao.FindById(userId);
        }

        virtual public User FindUser(String login)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, Helpers.TransActionOptions.ReadComitted))
            {
                return this.userDao.FindByLogin(login);
            }
        }

        virtual public Boolean UserExists(String login)
        {
            return (this.FindUser(login) != null);
        }

        virtual public Boolean ChangePassword(User user, String oldPassword, String newPassword)
        {
            if (user == null)
                return false;
            else
            {
                Boolean result = false;
                using (TransactionScope scope = new TransactionScope())
                {
                    if (user.ChangePassword(oldPassword, newPassword))
                    {
                        userDao.Save(user);
                        result = true;
                    }

                    scope.Complete();
                } return result;
            }
        }

        virtual public Boolean ChangePassword(String login, String oldPassword, String newPassword)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                Boolean succeeded = this.ChangePassword(
                             this.FindUser(login),
                             EncodePassword(oldPassword),
                             EncodePassword(newPassword)
                         );
                scope.Complete();
                return succeeded;
            }
        }

        public String ResetPassword(String login, String answer)
        {
            User user = this.FindUser(login);
            if (user == null)
                throw new UserNotFoundException(login);

            return this.ResetPassword(user, answer);
        }

        public String ResetPassword(User user, String answer)
        {
            if (!EnablePasswordReset)
            {
                throw new NotSupportedException("Password reset is not enabled.");
            }

            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(user, true);

                throw new UserServiceException("Password answer required for password reset.");
            }

            String newPassword =
              System.Web.Security.Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);

            String passwordAnswer;
            if (user != null)
            {
                if (user.IsLockedOut)
                    throw new UserServiceException("The supplied user is locked out.");

                passwordAnswer = user.PasswordAnswer;
            }
            else
            {
                throw new UserNotFoundException();
            }

            if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
            {
                UpdateFailureCount(user, true);

                throw new UserServiceException("Incorrect password answer.");
            }

            this.SetPassword(user, EncodePassword(newPassword));
            return newPassword;
        }

        virtual public void SetPassword(String login, String password)
        {
            User user = this.FindUser(login);
            if (user == null)
                throw new UserNotFoundException(login);

            this.SetPassword(user, password);
        }

        virtual public void SetPassword(User user, String password)
        {
            if (user == null)
                throw new UserNotFoundException();

            using (TransactionScope scope = new TransactionScope())
            {
                user.Password = password;
                user.LastPasswordChange = DateTime.Now;

                this.userDao.Save(user);
                scope.Complete();
            }
        }

        /// <summary>
        /// Changes the password question and answer of a user
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="password">the user's password</param>
        /// <param name="newPwdQuestion"></param>
        /// <param name="newPwdAnswer"></param>
        /// <returns>true if succeeded, false otherwise</returns>
        virtual public void ChangePasswordQuestionAndAnswer(User user, String newPwdQuestion, String newPwdAnswer)
        {
            if (user == null)
                throw new UserNotFoundException();

            //either you're an administrator or you're the user himself
            new PrincipalPermission(user.Login, null).Union(
                new PrincipalPermission(null, Steeg.Framework.Security.Roles.ADMINISTRATORS)
            ).Demand();

            using (TransactionScope scope = new TransactionScope())
            {
                user.PasswordQuestion = newPwdQuestion;
                user.PasswordAnswer = newPwdAnswer;

                this.userDao.Save(user);
                scope.Complete();
            }
        }

        public void ChangePasswordQuestionAndAnswer(String login, String newPwdQuestion, String newPwdAnswer)
        {
            this.ChangePasswordQuestionAndAnswer(
                this.FindUser(login), 
                newPwdQuestion, 
                EncodePassword(newPwdAnswer)
            );
        }

        virtual public User CreateUser(
            String login,
            String password,
            String email,
            String name,
            String passwordQuestion,
            String passwordAnswer,
            Boolean isApproved)
        {
            if (this.RequiresUniqueEmail && (this.FindLoginByEmail(email) != null))
            {
                throw new EmailInUseException(email);
            }

            if (!this.UserExists(login))
            {
                User user = null;
                using (TransactionScope scope = new TransactionScope())
                {
                    user = this.userDao.Create(
                                     new User(login, EncodePassword(password), email, name, passwordQuestion, EncodePassword(passwordAnswer), isApproved)
                                 );
                    scope.Complete();
                }
                return user;
            }
            else
            {
                throw new LoginInUseException(login);
            }
        }

        virtual public void UpdateUser(
            String login,
            String email,
            String name,
            Boolean isApproved)
        {
            User user = this.FindUser(login);
            if (user == null)
            {
                throw new UserNotFoundException(login);
            }
            else
            {
                this.UpdateUser(
                    user,
                    email,
                    name,
                    isApproved);
            }
        }

        virtual public void UpdateUser(
            User user,
            String email,
            String name,
            Boolean isApproved)
        {

            if (user == null)
            {
                throw new UserNotFoundException();
            }
            else
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    user.Email = email;
                    user.Name = name;
                    user.IsApproved = isApproved;

                    this.userDao.Save(user);
                    scope.Complete();
                }
            }
        }

        public void DeleteUser(String login)
        {
            User user = this.FindUser(login);
            if (user == null)
                throw new UserNotFoundException(login);
            this.DeleteUser(user);
        }

        virtual public void DeleteUser(User user)
        {
            if (user == null)
                throw new UserNotFoundException();

            //either you're an administrator or you're the user himself
            new PrincipalPermission(user.Login, null).Union(
                new PrincipalPermission(null, Steeg.Framework.Security.Roles.ADMINISTRATORS)
            ).Demand();

            using (TransactionScope scope = new TransactionScope())
            {
                this.userDao.Delete(user);
                scope.Complete();
            }
        }

        virtual public Int32 CountUsers()
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, Helpers.TransActionOptions.ReadComitted))
            {
                return this.userDao.CountAll();
            }
        }

        virtual public User[] GetAllUsers()
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, Helpers.TransActionOptions.ReadComitted))
            {
                return this.userDao.FindAll();
            }
        }

        virtual public User[] GetAllUsers(Int32 startRow, Int32 maxRows)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, Helpers.TransActionOptions.ReadComitted))
            {
                return this.userDao.FindAll(startRow, maxRows);
            }
        }

        virtual public Int32 CountOnlineUsers(TimeSpan activitySpan)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, Helpers.TransActionOptions.ReadComitted))
            {
                return this.userDao.CountOnlineUsers(activitySpan);
            }
        }

        virtual public void UpdateActivity(String login)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, Helpers.TransActionOptions.ReadComitted))
            {
                User user = this.FindUser(login);
                if (user == null)
                    throw new UserNotFoundException(login);

                this.UpdateActivity(user);
            }
        }

        virtual public void UpdateActivity(User user)
        {
            using (TransactionScope scope = new TransactionScope())
            {

                if (user == null)
                    throw new UserNotFoundException();

                user.LastActivity = DateTime.Now;

                this.userDao.Save(user);

                scope.Complete();
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = Security.Roles.ADMINISTRATORS)]
        virtual public void UnlockUser(User user)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                if (user == null)
                    throw new UserNotFoundException();
                else
                {
                    user.IsLockedOut = false;
                    user.LastLockedOut = DateTime.Now;

                    this.userDao.Save(user);
                }
                scope.Complete();
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = Security.Roles.ADMINISTRATORS)]
        virtual public void UnlockUser(String login)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                User user = this.FindUser(login);
                if (user == null)
                    throw new UserNotFoundException();
                else
                {
                    this.UnlockUser(user);
                }
                scope.Complete();
            }
        }

        public void LockUser(String login)
        {
            using (TransactionScope scope = new TransactionScope())
            {

                User user = this.FindUser(login);
                if (user == null)
                    throw new UserNotFoundException();
                else
                {
                    this.LockUser(user);
                }
                scope.Complete();
            }
        }

        virtual public void LockUser(User user)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                user.IsLockedOut = true;
                user.LastLockedOut = DateTime.Now;

                this.userDao.Save(user);
                scope.Complete();
            }
        }

        virtual public String GetPassword(String login, String answer)
        {
            if (!EnablePasswordRetrieval)
            {
                throw new UserServiceException("Password Retrieval Not Enabled.");
            }

            if (PasswordFormat == PasswordFormat.Hashed)
            {
                throw new UserServiceException("Cannot retrieve Hashed passwords.");
            }

            User user = this.FindUser(login);
            if (user != null)
            {
                return GetPassword(user, answer);
            }

            else
                throw new UserNotFoundException(login);

        }

        virtual public String GetPassword(User user, String answer)
        {
            if (!EnablePasswordRetrieval)
            {
                throw new UserServiceException("Password Retrieval Not Enabled.");
            }

            if (PasswordFormat == PasswordFormat.Hashed)
            {
                throw new UserServiceException("Cannot retrieve Hashed passwords.");
            }

            String password = null;
            String passwordAnswer = null;
            using (TransactionScope scope = new TransactionScope())
            {
                if (user != null)
                {
                    if (user.IsLockedOut)
                        throw new UserServiceException("The supplied user is locked out.");

                    password = user.Password;
                    passwordAnswer = user.PasswordAnswer;
                }
                else
                {
                    throw new UserNotFoundException();
                }

                if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
                {
                    UpdateFailureCount(user, true);

                    throw new UserServiceException("Incorrect password answer.");
                }

                if (PasswordFormat == PasswordFormat.Encrypted)
                {
                    password = UnEncodePassword(password);
                }

                scope.Complete();
            }
            return password;
        }

        virtual public String FindLoginByEmail(String email)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, Helpers.TransActionOptions.ReadComitted))
            {
                return this.userDao.FindLogin("Email", email);
            }
        }

        virtual public User[] FindUsersByPartialLogin(String login, Int32 startRow, Int32 maxRows)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, Helpers.TransActionOptions.ReadComitted))
            {
                return this.userDao.FindUsersByPartialPropertyValue("Login", login, startRow, maxRows);
            }
        }

        virtual public Int32 CountUsersByPartialLogin(String login)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, Helpers.TransActionOptions.ReadComitted))
            {
                return this.userDao.CountUsersByPartialPropertyValue("Login", login);
            }
        }

        virtual public User[] FindUsersByPartialEmail(String email, Int32 startRow, Int32 maxRows)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, Helpers.TransActionOptions.ReadComitted))
            {
                return this.userDao.FindUsersByPartialPropertyValue("Email", email, startRow, maxRows);
            }
        }

        virtual public Int32 CountUsersByPartialEmail(String email)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, Helpers.TransActionOptions.ReadComitted))
            {
                return this.userDao.CountUsersByPartialPropertyValue("Email", email);
            }
        }

        virtual public User Authenticate(String login, String password)
        {
            User user = null;
            using (TransactionScope scope = new TransactionScope())
            {
                password = this.EncodePassword(password);
                user = this.userDao.FindAuthorizedUser(login, password);
                if (user != null)
                {
                    user.LastLogin = DateTime.Now;
                    this.userDao.Update(user);
                }
                scope.Complete();
            }
            return user;
        }
        #endregion

        #region roles

        public void CreateRole(String roleName)
        {
            if (!this.RoleExists(roleName))
            {
                this.roleDao.Create(
                    new Role(roleName));
            }
            else
            {
                new RoleExistsException(roleName);
            }
        }

        virtual public Boolean RoleExists(String roleName)
        {
            return (FindRole(roleName) != null);
        }

        virtual public Role GetRole(Int64 id)
        {
            return this.roleDao.FindById(id);
        }

        virtual public Role FindRole(String roleName)
        {
            return this.roleDao.FindByName(roleName);
        }

        public void DeleteRole(String roleName)
        {
            Role role = FindRole(roleName);
            if (role == null)
                throw new RoleNotFoundException(roleName);
            else
                DeleteRole(role, false);
        }

        virtual public void DeleteRole(Role role)
        {
            if (role == null)
                throw new RoleNotFoundException();

            this.DeleteRole(role, false);
        }

        public void DeleteRole(String roleName, Boolean throwWhenUsed)
        {
            Role role = FindRole(roleName);
            if (role == null)
                throw new RoleNotFoundException(roleName);
            else
                DeleteRole(role, throwWhenUsed);
        }

        virtual public void DeleteRole(Role role, Boolean throwWhenUsed)
        {
            if (role == null)
                throw new RoleNotFoundException();

            //TODO: optimize instead of role.users.count
            if (throwWhenUsed && role.Users.Count > 0)
            {
                throw new RoleInUseException(role);
            }
            
            this.roleDao.Delete(role);
        }


        virtual public void AddUsersToRoles(ICollection<User> users, ICollection<Role> roles)
        {
            if ((users == null) || (roles == null))
                return;

            using (TransactionScope scope = new TransactionScope())
            {
                foreach (User u in users)
                {
                    foreach (Role role in roles)
                    {
                        u.Roles.Add(role);
                    }
                    this.userDao.Save(u);
                }
                scope.Complete();
            }
        }

        public void AddUsersToRoles(ICollection<String> logins, ICollection<String> roleNames)
        {
            User[] users = this.userDao.FindByLogin(logins);
            if (users == null || users.Length != logins.Count)
                throw new UserNotFoundException();

            Role[] roles = this.roleDao.FindByName(roleNames);
            if (roles == null || roles.Length != roleNames.Count)
                throw new RoleNotFoundException();
            
            this.AddUsersToRoles(users,roles);
        }

        public void RemoveUsersFromRoles(ICollection<String> logins, ICollection<String> roleNames)
        {
            User[] users = this.userDao.FindByLogin(logins);
            if (users == null || users.Length != logins.Count)
                throw new UserNotFoundException();

            Role[] roles = this.roleDao.FindByName(roleNames);
            if (roles == null || roles.Length != roleNames.Count)
                throw new RoleNotFoundException();

            this.RemoveUsersFromRoles(users, roles);
        }

        virtual public void RemoveUsersFromRoles(ICollection<User> users, ICollection<Role> roles)
        {
            if ((users == null) || (roles == null))
                return;

            using (TransactionScope scope = new TransactionScope())
            {
                foreach (User u in users)
                {
                    foreach (Role role in roles)
                    {
                        u.Roles.Remove(role);
                    }
                    this.userDao.Save(u);
                }
                scope.Complete();
            }
        }

        virtual public Role[] GetAllRoles()
        {
            return this.roleDao.FindAll();
        }
        #endregion

        //
        // EncodePassword
        //   Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        //
        private String EncodePassword(String password)
        {
            if (password == null)
                return null;

            String encodedPassword = password;
            switch (PasswordFormat)
            {
                case PasswordFormat.Clear:
                    break;
                case PasswordFormat.Encrypted:
                    encodedPassword =
                      Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case PasswordFormat.Hashed:
                    encodedPassword = Helpers.Encryption.MD5(password);
                    break;
                default:
                    throw new UserServiceException("Unsupported password format.");
            }

            return encodedPassword;
        }


        //
        // UnEncodePassword
        //   Decrypts or leaves the password clear based on the PasswordFormat.
        //
        private String UnEncodePassword(String encodedPassword)
        {
            String password = encodedPassword;

            switch (PasswordFormat)
            {
                case PasswordFormat.Clear:
                    break;
                case PasswordFormat.Encrypted:
                    password =
                      Encoding.Unicode.GetString(
                        DecryptPassword(Convert.FromBase64String(password))
                      );
                    break;
                case PasswordFormat.Hashed:
                    throw new UserServiceException("Cannot unencode a hashed password.");
                default:
                    throw new UserServiceException("Unsupported password format.");
            }

            return password;
        }

        SymmetricAlgorithm GetAlg(out byte[] decryptionKey)
        {
            MachineKeySection section = (MachineKeySection)WebConfigurationManager.GetSection("system.web/machineKey");

            if (section.DecryptionKey.StartsWith("AutoGenerate"))
                throw new UserServiceException("You must explicitly specify a decryption key in the <machineKey> section when using encrypted passwords.");

            String alg_type = section.Decryption;
            if (alg_type == "Auto")
                alg_type = "AES";

            SymmetricAlgorithm alg = null;
            if (alg_type == "AES")
                alg = Rijndael.Create();
            else if (alg_type == "3DES")
                alg = TripleDES.Create();
            else
                throw new UserServiceException(String.Format("Unsupported decryption attribute '{0}' in <machineKey> configuration section", alg_type));

            decryptionKey = System.Text.Encoding.Unicode.GetBytes(section.DecryptionKey);
            return alg;
        }

        protected byte[] DecryptPassword(byte[] encodedPassword)
        {
            byte[] decryptionKey;

            using (SymmetricAlgorithm alg = GetAlg(out decryptionKey))
            {
                alg.Key = decryptionKey;

                using (ICryptoTransform decryptor = alg.CreateDecryptor())
                {

                    byte[] buf = decryptor.TransformFinalBlock(encodedPassword, 0, encodedPassword.Length);
                    byte[] rv = new byte[buf.Length - SALT_BYTES];

                    Array.Copy(buf, 16, rv, 0, buf.Length - 16);
                    return rv;
                }
            }
        }

        protected byte[] EncryptPassword(byte[] password)
        {
            byte[] decryptionKey;
            byte[] iv = new byte[SALT_BYTES];

            Array.Copy(password, 0, iv, 0, SALT_BYTES);
            Array.Clear(password, 0, SALT_BYTES);

            using (SymmetricAlgorithm alg = GetAlg(out decryptionKey))
            {
                using (ICryptoTransform encryptor = alg.CreateEncryptor(decryptionKey, iv))
                {
                    return encryptor.TransformFinalBlock(password, 0, password.Length);
                }
            }
        }

        //
        // UpdateFailureCount
        //  
        /// <summary>
        /// A helper method that performs the checks and updates associated with
        /// password failure tracking.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="isPasswordAnswer">true if the passwordanswer has failed, false if password was incorrect</param>
        protected void UpdateFailureCount(User user, Boolean isPasswordAnswer)
        {
            DateTime windowStart = new DateTime();
            Int32 failureCount = 0;

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            if (isPasswordAnswer)
            {
                failureCount = user.FailedPasswordAnswerAttemptCount;
                windowStart = user.FailedPasswordAnswerAttemptWindowStart;
            }
            else //password
            {
                failureCount = user.FailedPasswordAttemptCount;
                windowStart = user.FailedPasswordAttemptWindowStart;
            }

            DateTime windowEnd = windowStart.AddMinutes(PasswordAttemptWindowMinutes);

            using (TransactionScope scope = new TransactionScope())
            {
                if (failureCount == 0 || DateTime.Now > windowEnd)
                {
                    // First password failure or outside of PasswordAttemptWindow. 
                    // Start a new password failure count from 1 and a new window starting now.
                    if (isPasswordAnswer)
                    {
                        user.FailedPasswordAttemptCount = 1;
                        user.FailedPasswordAttemptWindowStart = DateTime.Now;
                    }
                    else //password
                    {
                        user.FailedPasswordAttemptCount = 1;
                        user.FailedPasswordAttemptWindowStart = DateTime.Now;
                    }
                }
                else
                {
                    if (failureCount++ >= MaxInvalidPasswordAttempts)
                    {
                        // Password attempts have exceeded the failure threshold. Lock out
                        // the user.
                        user.IsLockedOut = true;
                        user.LastLockedOut = DateTime.Now;
                    }
                    else
                    {
                        // Password attempts have not exceeded the failure threshold. Update
                        // the failure counts. Leave the window the same.
                        if (isPasswordAnswer)
                        {
                            user.FailedPasswordAnswerAttemptCount = failureCount;
                        }
                        else //password
                        {
                            user.FailedPasswordAttemptCount = failureCount;
                        }
                    }
                }

                this.userDao.Save(user);
                scope.Complete();
            }
        }

        //
        // CheckPassword
        //   Compares password values based on the MembershipPasswordFormat.
        //
        private Boolean CheckPassword(String password, String dbpassword)
        {
            switch (PasswordFormat)
            {
                case PasswordFormat.Encrypted:
                    dbpassword = UnEncodePassword(dbpassword);
                    break;
                case PasswordFormat.Hashed:
                    password = EncodePassword(password);
                    break;
                default:
                    break;
            }

            return (password == dbpassword);
        }

    }
}
