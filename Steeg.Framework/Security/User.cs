namespace Steeg.Framework.Security
{
    using System;
    using System.Collections.Generic;
    using System.Security.Principal;

    public class User : IPrincipal
    {
        public User() { }
        public User(
                 string login,
                 string password,
                 string email,
                 string name,
                 string passwordQuestion,
                 string passwordAnswer,
                 bool isApproved)
        {
            this.login = login;
            this.password = password;
            this.email = email;
            this.name = name;
            this.passwordQuestion = passwordQuestion;
            this.passwordAnswer = passwordAnswer;
            this.isApproved = isApproved;
        }


        #region properties
        private Int64 id;
        public Int64 Id
        {
            get { return id; }
            set { id = value; }
        }

        private String login;
        public String Login
        {
            get { return login; }
            set { login = value; }
        }

        private String name;
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        private String email;
        public String Email
        {
            get { return email; }
            set { email = value; }
        }

        private String password;
        public String Password
        {
            get { return password; }
            set { password = value; }
        }

        private Boolean isApproved;
        public Boolean IsApproved
        {
            get { return isApproved; }
            set { isApproved = value; }
        }

        private DateTime lastActivity;
        public DateTime LastActivity
        {
            get { return lastActivity; }
            set { lastActivity = value; }
        }

        private DateTime lastLogin;
        public DateTime LastLogin
        {
            get { return lastLogin; }
            set { lastLogin = value; }
        }

        private DateTime lastPasswordChange;
        public DateTime LastPasswordChange
        {
            get { return lastPasswordChange; }
            set { lastPasswordChange = value; }
        }

        private DateTime creationDate = DateTime.Now;
        public DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }

        private Boolean isOnLine;
        public Boolean IsOnLine
        {
            get { return isOnLine; }
            set { isOnLine = value; }
        }

        private Boolean isLockedOut;
        public Boolean IsLockedOut
        {
            get { return isLockedOut; }
            set { isLockedOut = value; }
        }

        private DateTime lastLockedOut;
        public DateTime LastLockedOut
        {
            get { return lastLockedOut; }
            set { lastLockedOut = value; }
        }

        private Int32 failedPasswordAttemptCount;
        public Int32 FailedPasswordAttemptCount
        {
            get { return failedPasswordAttemptCount; }
            set { failedPasswordAttemptCount = value; }
        }

        private DateTime failedPasswordAttemptWindowStart;
        public DateTime FailedPasswordAttemptWindowStart
        {
            get { return failedPasswordAttemptWindowStart; }
            set { failedPasswordAttemptWindowStart = value; }
        }

        private Int32 failedPasswordAnswerAttemptCount;
        public Int32 FailedPasswordAnswerAttemptCount
        {
            get { return failedPasswordAnswerAttemptCount; }
            set { failedPasswordAnswerAttemptCount = value; }
        }

        private DateTime failedPasswordAnswerAttemptWindowStart;
        public DateTime FailedPasswordAnswerAttemptWindowStart
        {
            get { return failedPasswordAnswerAttemptWindowStart; }
            set { failedPasswordAnswerAttemptWindowStart = value; }
        }

        private String passwordQuestion;
        public String PasswordQuestion
        {
            get { return passwordQuestion; }
            set { passwordQuestion = value; }
        }

        private String passwordAnswer;
        public String PasswordAnswer
        {
            get { return passwordAnswer; }
            set { passwordAnswer = value; }
        }

        #endregion

        public Boolean IsInRole(String role)
        {
            //TODO: If it's ever decided that a model-value object can have a reference to the service, we should use the service to perform a single query for this
            foreach (Role r in roles)
            {
                if (r.RoleName.Equals(role, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public IIdentity Identity
        {
            get { return new GenericIdentity(login, "steeg.framework.authentication"); }
        }

        private IList<Role> roles = new List<Role>();
        public IList<Role> Roles
        {
            get { return roles; }
            set { roles = value; }
        }


        /// <summary>
        /// Changes the user's password if the old one given is correct
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public bool ChangePassword(string oldPassword, string newPassword)
        {
            if (this.Password.Equals(oldPassword))
            {
                this.Password = newPassword;
                this.LastPasswordChange = DateTime.Now;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return this.Login;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            else if (obj is String)
                return ((String)obj).Equals(this.Login, StringComparison.CurrentCultureIgnoreCase);
            else if (obj is User)
                return ((User)obj) == this;
            else
                return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            if (this.Login == null)
                return 0;
            else
                return this.Login.ToLower().GetHashCode();
        }

        public static bool operator ==(User user1, User user2)
        {
            if (user1 == null)
                return (user2 == null);
            else if (user2 == null)
                return false;
            else if (user1.Login == null)
                return user2.Login == null;
            else
                return (user1.Login.Equals(user2.Login, StringComparison.CurrentCultureIgnoreCase));
        }

        public static bool operator !=(User user1, User user2)
        {
            if (user1 == null)
                return (user2 != null);
            else if (user2 == null)
                return true;
            else if (user1.Login == null)
                return user2.Login != null;
            else
                return (!user1.Login.Equals(user2.Login, StringComparison.CurrentCultureIgnoreCase));
        }

        public System.Web.Security.MembershipUser ToMembershipUser()
        {
            string provName = typeof(Steeg.Framework.Web.Security.SteegFrameworkMemberShipProvider).Name;
            if (System.Web.Security.Membership.Provider != null)
                provName = System.Web.Security.Membership.Provider.Name;
            return new System.Web.Security.MembershipUser(
                provName,
                    //TODO: Find out a reason why you would ever want to use a different providername then the default one, is it possible to switch provider every now and then ??
                this.Login,
                this.Id,
                this.Email,
                this.PasswordQuestion,
                this.Name,
                this.IsApproved,
                this.IsLockedOut,
                this.CreationDate,
                this.LastLogin,
                this.LastActivity,
                this.LastPasswordChange,
                this.LastLockedOut);
        }

        /// <summary>
        /// Allows magically converting of our own users to ASP.NET membership users.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static implicit operator System.Web.Security.MembershipUser(User user)
        {
            if (user == null)
                return null;
            else
                return user.ToMembershipUser();
        }

        public static implicit operator String(User user)
        {
            if (user == null)
                return null;
            else
                return user.ToString();
        }
        
        public static String[] ToString(IList<User> users)
        {
            if (users == null)
                return null;

            String[] userNames = new String[users.Count];
            for (int i = 0; i < users.Count; i++)
            {
                userNames[i] = users[i].Login;
            }
            return userNames;
        }
    }
}
