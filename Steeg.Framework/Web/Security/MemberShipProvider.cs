using System.Web.Security;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System;
using System.Data;
using System.Data.Odbc;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Globalization;
using System.Text;
using System.Web.Configuration;

using Steeg.Framework.Security;

using Castle.Windsor;

namespace Steeg.Framework.Web.Security
{
    public sealed class SteegFrameworkMemberShipProvider: MembershipProvider
    {
        private string eventSource = "SteegFrameworkMembershipProvider";
        private string eventLog = "Application";
        private string exceptionMessage = "An exception occurred. Please check the Event Log.";
        private IUserService userService;

        //
        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        //
        private bool writeExceptionsToEventLog;
        public bool WriteExceptionsToEventLog
        {
            get { return writeExceptionsToEventLog; }
            set { writeExceptionsToEventLog = value; }
        }


        //
        // System.Configuration.Provider.ProviderBase.Initialize Method
        //
        public override void Initialize(string name, NameValueCollection config)
        {
            IWindsorContainer container = SteegWindsorContainer.Obtain();
            
            this.userService =
                container[typeof(IUserService)] as IUserService;

            if (this.userService == null)
                throw new ProviderException("SteegFramework MemberShipProvider only works when used in combination with an IUserService. You should add this component to your container.");
            
            //
            // Initialize values from web.config.
            //

            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = typeof(SteegFrameworkMemberShipProvider).Name;

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Steeg framework Membership provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            applicationName = GetConfigValue(config["applicationName"],
                                            System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            
            writeExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));
        }

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        private MembershipUserCollection CreateMembershipUserCollection(User[] users)
        {
            if ((users == null) || (users.Length == 0))
                return new MembershipUserCollection();

            MembershipUserCollection c = new MembershipUserCollection();
            foreach (User u in users)
            {
                c.Add(u);
            }

            return c;
            
        }

        //
        // System.Web.Security.MembershipProvider properties.
        //
        private string applicationName;
        public override string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        public override bool EnablePasswordReset
        {
            get { return this.userService.EnablePasswordReset; }
        }


        public override bool EnablePasswordRetrieval
        {
            get { return this.userService.EnablePasswordRetrieval; }
        }


        public override bool RequiresQuestionAndAnswer
        {
            get { return this.userService.RequiresQuestionAndAnswer; }
        }


        public override bool RequiresUniqueEmail
        {
            get { return this.userService.RequiresUniqueEmail; }
        }


        public override int MaxInvalidPasswordAttempts
        {
            get { return this.userService.MaxInvalidPasswordAttempts; }
        }


        public override int PasswordAttemptWindow
        {
            get { return this.userService.PasswordAttemptWindowMinutes; }
        }


        public override MembershipPasswordFormat PasswordFormat
        {
            get 
            {
                switch (userService.PasswordFormat)
                {
                    case Steeg.Framework.Security.PasswordFormat.Hashed:
                        return MembershipPasswordFormat.Hashed;
                    case Steeg.Framework.Security.PasswordFormat.Encrypted:
                        return MembershipPasswordFormat.Encrypted;
                    case Steeg.Framework.Security.PasswordFormat.Clear:
                        return MembershipPasswordFormat.Clear;
                    default:
                        throw new ProviderException("Password format not supported.");
                }
            }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return this.userService.MinRequiredNonAlphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return this.userService.MinRequiredPasswordLength; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return this.userService.PasswordStrengthRegularExpression; }
        }

        //
        // System.Web.Security.MembershipProvider methods.
        //

        //
        // MembershipProvider.ChangePassword
        //

        public override bool ChangePassword(string username, string oldPwd, string newPwd)
        {
            if (!ValidateUser(username, oldPwd))
                return false;

            ValidatePasswordEventArgs args =
              new ValidatePasswordEventArgs(username, newPwd, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");
            }
            else
            {
                return userService.ChangePassword(username, oldPwd, newPwd);
            }
        }



        //
        // MembershipProvider.ChangePasswordQuestionAndAnswer
        //

        public override bool ChangePasswordQuestionAndAnswer(string username,
                      string password,
                      string newPwdQuestion,
                      string newPwdAnswer)
        {
            if (!ValidateUser(username, password))
                return false;

            try
            {
                this.userService.ChangePasswordQuestionAndAnswer(
                    username, newPwdQuestion, newPwdAnswer);

                return true;
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "CreateUser");
                }

                return false;
            }
        }



        //
        // MembershipProvider.CreateUser
        //
        public override MembershipUser CreateUser(string username,
                 string password,
                 string email,
                 string passwordQuestion,
                 string passwordAnswer,
                 bool isApproved,
                 object providerUserKey,
                 out MembershipCreateStatus status)
        {
            ValidatePasswordEventArgs args =
              new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (providerUserKey != null)
            {
                //we don't support id's created by someone else
                status = MembershipCreateStatus.InvalidProviderUserKey;
                return null;
            }

            try
            {
                User user = this.userService.CreateUser(
                    username,
                    password,
                    email,
                    null,
                    passwordQuestion,
                    passwordAnswer,
                    isApproved);

                if (user == null)
                {
                    status = MembershipCreateStatus.UserRejected;
                }
                else
                {
                    status = MembershipCreateStatus.Success;
                }

                return user;

            }
            catch (LoginInUseException)
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }
            catch (EmailInUseException)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "CreateUser");
                }

                status = MembershipCreateStatus.ProviderError;
                return null;
            }
        }


        //
        // MembershipProvider.DeleteUser
        //
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            try
            {
                this.userService.DeleteUser(username);
                return true;
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "DeleteUser");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
        }



        //
        // MembershipProvider.GetAllUsers
        //

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            try
            {
                totalRecords = this.userService.CountUsers();
                Int32 start = pageIndex * pageSize;
                if ((totalRecords > 0) && (totalRecords > start))
                {
                    return
                        this.CreateMembershipUserCollection(
                            this.userService.GetAllUsers(start, pageSize)
                        );
                }
                else
                {
                    return new MembershipUserCollection();
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetAllUsers");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
        }


        //
        // MembershipProvider.GetNumberOfUsersOnline
        //

        public override int GetNumberOfUsersOnline()
        {
            try
            {
                TimeSpan onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
                return this.userService.CountOnlineUsers(onlineSpan);
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetNumberOfUsersOnline");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
        }

        //
        // MembershipProvider.GetPassword
        //

        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval)
            {
                throw new ProviderException("Password Retrieval Not Enabled.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                throw new ProviderException("Cannot retrieve Hashed passwords.");
            }

            try
            {
                return this.userService.GetPassword(username, answer);
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetPassword");

                }
                throw new MembershipPasswordException(e.Message, e);
            }
        }



        //
        // MembershipProvider.GetUser(string, bool)
        //
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            try
            {
                User user = this.userService.FindUser(username);
                if (user != null)
                {
                    if (userIsOnline)
                    {
                        this.userService.UpdateActivity(user);
                    }
                }

                return user;

            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(String, Boolean)");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
        }


        //
        // MembershipProvider.GetUser(object, bool)
        //

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (!(providerUserKey is Int64))
                throw new ProviderException("We only use Int64(long) keys for the users");

            try
            {
                User user = this.userService.GetUser((Int64)providerUserKey);
                
                if (user != null)
                {
                    if (userIsOnline)
                    {
                        this.userService.UpdateActivity(user);
                    }
                }

                return user;
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(Object, Boolean)");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
        }


        //
        // MembershipProvider.UnlockUser
        //
        public override bool UnlockUser(string username)
        {
            try
            {
                this.userService.UnlockUser(username);
                return true;
            }
            catch (UserNotFoundException)
            {
                return false;
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UnlockUser");

                    throw new ProviderException(exceptionMessage, e);
                }
                else
                {
                    throw;
                }
            }
        }


        //
        // MembershipProvider.GetUserNameByEmail
        //

        public override string GetUserNameByEmail(string email)
        {
            try
            {
                string username = this.userService.FindLoginByEmail(email);
                if (username == null)
                    username = String.Empty;

                return username;
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUserNameByEmail");

                    throw new ProviderException(exceptionMessage, e);
                }
                else
                {
                    throw;
                }
            }
        }

        //
        // MembershipProvider.ResetPassword
        //
        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset)
            {
                throw new NotSupportedException("Password reset is not enabled.");
            }

            try
            {
                return this.userService.ResetPassword(username, answer);
            }
            catch (UserNotFoundException e)
            {
                throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.", e);
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ResetPassword");
                }
                throw new ProviderException(exceptionMessage, e);

            }
        }


        //
        // MembershipProvider.UpdateUser
        //
        public override void UpdateUser(MembershipUser user)
        {
            try
            {
                this.userService.UpdateUser(
                    user.UserName,
                    user.Email,
                    user.Comment,
                    user.IsApproved);
            }
            catch (UserNotFoundException ex)
            {
                throw new ProviderException("User was not found, cannot update the user", ex);
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UpdateUser");

                    throw new ProviderException(exceptionMessage, e);
                }
                else
                {
                    throw;
                }
            }
        }


        //
        // MembershipProvider.ValidateUser
        //

        public override bool ValidateUser(string username, string password)
        {
            try
            {
                User user = this.userService.Authenticate(username, password);
                if (user == null)
                    return false;
                else
                    return true;
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ValidateUser");

                    throw new ProviderException(exceptionMessage, e);
                }
                else
                {
                    throw;
                }
            }
        }


        //
        // MembershipProvider.FindUsersByName
        //
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            int startIndex = pageSize * pageIndex;

            try
            {
                totalRecords = this.userService.CountUsersByPartialLogin(usernameToMatch);
                if (totalRecords > startIndex)
                {
                    return CreateMembershipUserCollection(
                        this.userService.FindUsersByPartialLogin(usernameToMatch, startIndex, pageSize)
                    );
                }
                else
                {
                    return new MembershipUserCollection();
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "FindUsersByName");

                    throw new ProviderException(exceptionMessage, e);
                }
                else
                {
                    throw;
                }
            }
            
        }

        //
        // MembershipProvider.FindUsersByEmail
        //
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            int startIndex = pageSize * pageIndex;
            
            try
            {
                totalRecords = this.userService.CountUsersByPartialEmail(emailToMatch);

                if (totalRecords > startIndex)
                {
                    return CreateMembershipUserCollection(
                        this.userService.FindUsersByPartialEmail(emailToMatch, startIndex, pageSize)
                    );
                }
                else
                {
                    return new MembershipUserCollection();
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "FindUsersByEmail");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
        }


        //
        // WriteToEventLog
        //   A helper function that writes exception detail to the event log. Exceptions
        // are written to the event log as a security measure to avoid private database
        // details from being returned to the browser. If a method does not return a status
        // or boolean indicating the action succeeded or failed, a generic exception is also 
        // thrown by the caller.
        //

        private void WriteToEventLog(Exception e, string action)
        {
            EventLog log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;

            string message = "An exception occurred communicating with the data source.\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e.ToString();

            log.WriteEntry(message);
        }

    }
}