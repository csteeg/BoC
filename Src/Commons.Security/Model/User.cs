using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Web.DomainServices;
using BoC.Persistence;

namespace BoC.Security.Model
{
    public class User : BaseEntity<long>, IPrincipal
    {
        public User()
        {
            CreationDate = DateTime.Now;
        }

        public User(
            string login,
            string password,
            string email,
            string name,
            string passwordQuestion,
            string passwordAnswer,
            bool isApproved)
        {
            CreationDate = DateTime.Now;
            
            this.Login = login;
            this.Password = password;
            this.Email = email;
            this.Name = name;
            this.PasswordQuestion = passwordQuestion;
            this.PasswordAnswer = passwordAnswer;
            this.IsApproved = isApproved;
        }

        #region properties
        [Required]
        virtual public string Login { get; set; }

        virtual public String Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        virtual public String Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        virtual public String Password { get; set; }

        virtual public Boolean IsApproved { get; set; }

        virtual public DateTime? LastActivity { get; set; }

        virtual public DateTime? LastLogin { get; set; }

        virtual public DateTime? LastPasswordChange { get; set; }

        virtual public DateTime? CreationDate { get; set; }

        virtual public Boolean IsOnLine { get; set; }

        virtual public Boolean IsLockedOut { get; set; }

        virtual public DateTime? LastLockedOut { get; set; }

        virtual public Int32 FailedPasswordAttemptCount { get; set; }

        virtual public DateTime? FailedPasswordAttemptWindowStart { get; set; }

        virtual public Int32 FailedPasswordAnswerAttemptCount { get; set; }

        virtual public DateTime? FailedPasswordAnswerAttemptWindowStart { get; set; }

        virtual public String PasswordQuestion { get; set; }

        virtual public String PasswordAnswer { get; set; }

        #endregion

        virtual public Boolean IsInRole(String role)
        {
            //TODO: If it's ever decided that a model object can have a reference to the service, we should use the service to perform a single query for this
            foreach (Role r in roles)
            {
                if (r.RoleName.Equals(role, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        [ScaffoldColumn(false), Exclude]
        virtual public IIdentity Identity
        {
            get { return new GenericIdentity(Login, "steeg.framework.authentication"); }
        }

        private ICollection<Role> roles = new HashSet<Role>();
        virtual public ICollection<Role> Roles
        {
            get { return roles; }
            protected set { roles = value; }
        }


        /// <summary>
        /// Changes the user's password if the old one given is correct
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        virtual public bool ChangePassword(string oldPassword, string newPassword)
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

        virtual public System.Web.Security.MembershipUser ToMembershipUser()
        {
            string provName = "BoC.usermembership";
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
                this.CreationDate.HasValue ? this.CreationDate.Value : DateTime.Now,
                this.LastLogin.HasValue ? this.LastLogin.Value : DateTime.MinValue,
                this.LastActivity.HasValue ? this.LastActivity.Value : DateTime.MinValue,
                this.LastPasswordChange.HasValue ? this.LastPasswordChange.Value : DateTime.MinValue,
                this.LastLockedOut.HasValue ? this.LastLockedOut.Value : DateTime.MinValue);
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