using System.Web.Security;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System;
using System.Data;
using System.Data.SQLite;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;


namespace Mascix.SQLiteProviders
{

    public sealed class SQLiteMembershipProvider : MembershipProvider
    {

      

        private int newPasswordLength = 8;
        private string eventSource = "SQLiteMembershipProvider";
        private string eventLog = "Application";
        private string exceptionMessage = "An exception occurred. Please check the Event Log.";
        private string tableName = "users";
        private string connectionString;

        private const string encryptionKey = "AE09F72BA97CBBB5";

        //
        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        //

        private bool pWriteExceptionsToEventLog;

        public bool WriteExceptionsToEventLog
        {
            get { return pWriteExceptionsToEventLog; }
            set { pWriteExceptionsToEventLog = value; }
        }


        //
        // System.Configuration.Provider.ProviderBase.Initialize Method
        //

        public override void Initialize(string name, NameValueCollection config)
        {
            //
            // Initialize values from web.config.
            //

            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "SQLiteMembershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Sample SQLite Membership provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            pApplicationName = GetConfigValue(config["applicationName"],
                                            System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
            pWriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));

            string temp_format = config["passwordFormat"];
            if (temp_format == null)
            {
                temp_format = "Hashed";
            }

            switch (temp_format)
            {
                case "Hashed":
                    pPasswordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    pPasswordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    pPasswordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }

            //
            // Initialize SQLiteConnection.
            //

            ConnectionStringSettings ConnectionStringSettings =
              ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

            if (ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim() == "")
            {
                throw new ProviderException("Connection string cannot be blank.");
            }

            connectionString = ConnectionStringSettings.ConnectionString;


        }


        //
        // A helper function to retrieve config values from the configuration file.
        //

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }


        //
        // System.Web.Security.MembershipProvider properties.
        //


        private string pApplicationName;
        private bool pEnablePasswordReset;
        private bool pEnablePasswordRetrieval;
        private bool pRequiresQuestionAndAnswer;
        private bool pRequiresUniqueEmail;
        private int pMaxInvalidPasswordAttempts;
        private int pPasswordAttemptWindow;
        private MembershipPasswordFormat pPasswordFormat;

        public override string ApplicationName
        {
            get { return pApplicationName; }
            set { pApplicationName = value; }
        }

        public override bool EnablePasswordReset
        {
            get { return pEnablePasswordReset; }
        }


        public override bool EnablePasswordRetrieval
        {
            get { return pEnablePasswordRetrieval; }
        }


        public override bool RequiresQuestionAndAnswer
        {
            get { return pRequiresQuestionAndAnswer; }
        }


        public override bool RequiresUniqueEmail
        {
            get { return pRequiresUniqueEmail; }
        }


        public override int MaxInvalidPasswordAttempts
        {
            get { return pMaxInvalidPasswordAttempts; }
        }


        public override int PasswordAttemptWindow
        {
            get { return pPasswordAttemptWindow; }
        }


        public override MembershipPasswordFormat PasswordFormat
        {
            get { return pPasswordFormat; }
        }

        private int pMinRequiredNonAlphanumericCharacters;

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return pMinRequiredNonAlphanumericCharacters; }
        }

        private int pMinRequiredPasswordLength;

        public override int MinRequiredPasswordLength
        {
            get { return pMinRequiredPasswordLength; }
        }

        private string pPasswordStrengthRegularExpression;

        public override string PasswordStrengthRegularExpression
        {
            get { return pPasswordStrengthRegularExpression; }
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
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");


            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand("UPDATE `" + tableName + "`" +
                    " SET Password = $Password, LastPasswordChangedDate = $LastPasswordChangedDate " +
                    " WHERE Username = $Username AND ApplicationName = $ApplicationName", conn);

            cmd.Parameters.Add("$Password", DbType.String, 255).Value = EncodePassword(newPwd);
            cmd.Parameters.Add("$LastPasswordChangedDate", DbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username;
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;


            int rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ChangePassword");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }

            if (rowsAffected > 0)
            {
                return true;
            }

            return false;
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

            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand("UPDATE `" + tableName + "`" +
                    " SET PasswordQuestion = $PasswordQuestion, PasswordAnswer = $PasswordAnswer" +
                    " WHERE Username = $Username AND ApplicationName = $ApplicationName", conn);

            cmd.Parameters.Add("$Question", DbType.String, 255).Value = newPwdQuestion;
            cmd.Parameters.Add("$Answer", DbType.String, 255).Value = EncodePassword(newPwdAnswer);
            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username;
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;


            int rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ChangePasswordQuestionAndAnswer");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }

            if (rowsAffected > 0)
            {
                return true;
            }

            return false;
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



            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            MembershipUser u = GetUser(username, false);

            if (u == null)
            {
                DateTime createDate = DateTime.Now;

                if (providerUserKey == null)
                {
                    providerUserKey = Guid.NewGuid();
                }
                else
                {
                    if (!(providerUserKey is Guid))
                    {
                        status = MembershipCreateStatus.InvalidProviderUserKey;
                        return null;
                    }
                }

                SQLiteConnection conn = new SQLiteConnection(connectionString);
                SQLiteCommand cmd = new SQLiteCommand("INSERT INTO `" + tableName + "`" +
                      " (PKID, Username, Password, Email, PasswordQuestion, " +
                      " PasswordAnswer, IsApproved," +
                      " Comment, CreationDate, LastPasswordChangedDate, LastActivityDate," +
                      " ApplicationName, IsLockedOut, LastLockedOutDate," +
                      " FailedPasswordAttemptCount, FailedPasswordAttemptWindowStart, " +
                      " FailedPasswordAnswerAttemptCount, FailedPasswordAnswerAttemptWindowStart)" +
                      " Values($PKID, $Username, $Password, $Email, $PasswordQuestion, " +
                      " $PasswordAnswer, $IsApproved, $Comment, $CreationDate, $LastPasswordChangedDate, " +
                      " $LastActivityDate, $ApplicationName, $IsLockedOut, $LastLockedOutDate, " + 
                      " $FailedPasswordAttemptCount, $FailedPasswordAttemptWindowStart, " +
                      " $FailedPasswordAnswerAttemptCount, $FailedPasswordAnswerAttemptWindowStart)", conn);

                cmd.Parameters.Add("$PKID", DbType.String).Value = providerUserKey.ToString();
                cmd.Parameters.Add("$Username", DbType.String, 255).Value = username;
                cmd.Parameters.Add("$Password", DbType.String, 255).Value = EncodePassword(password);
                cmd.Parameters.Add("$Email", DbType.String, 128).Value = email;
                cmd.Parameters.Add("$PasswordQuestion", DbType.String, 255).Value = passwordQuestion;
                cmd.Parameters.Add("$PasswordAnswer", DbType.String, 255).Value = EncodePassword(passwordAnswer);
                cmd.Parameters.Add("$IsApproved", DbType.Boolean).Value = isApproved;
                cmd.Parameters.Add("$Comment", DbType.String, 255).Value = "";
                cmd.Parameters.Add("$CreationDate", DbType.DateTime).Value = createDate;
                cmd.Parameters.Add("$LastPasswordChangedDate", DbType.DateTime).Value = createDate;
                cmd.Parameters.Add("$LastActivityDate", DbType.DateTime).Value = createDate;
                cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;
                cmd.Parameters.Add("$IsLockedOut", DbType.Boolean).Value = false;
                cmd.Parameters.Add("$LastLockedOutDate", DbType.DateTime).Value = createDate;
                cmd.Parameters.Add("$FailedPasswordAttemptCount", DbType.Int32).Value = 0;
                cmd.Parameters.Add("$FailedPasswordAttemptWindowStart", DbType.DateTime).Value = createDate;
                cmd.Parameters.Add("$FailedPasswordAnswerAttemptCount", DbType.Int32).Value = 0;
                cmd.Parameters.Add("$FailedPasswordAnswerAttemptWindowStart", DbType.DateTime).Value = createDate;

                try
                {
                    conn.Open();

                    int recAdded = cmd.ExecuteNonQuery();

                    if (recAdded > 0)
                    {
                        status = MembershipCreateStatus.Success;
                    }
                    else
                    {
                        status = MembershipCreateStatus.UserRejected;
                    }
                }
                catch (SQLiteException e)
                {
                    if (WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(e, "CreateUser");
                    }

                    status = MembershipCreateStatus.ProviderError;
                }
                finally
                {
                    conn.Close();
                }


                return GetUser(username, false);
            }
            else
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }


            return null;
        }



        //
        // MembershipProvider.DeleteUser
        //

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand("DELETE FROM `" + tableName + "`" +
                    " WHERE Username = $Username AND ApplicationName = $ApplicationName", conn);

            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username;
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;

            int rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();

                if (deleteAllRelatedData)
                {
                    // Process commands to delete all data for the user in the database.
                }
            }
            catch (SQLiteException e)
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
            finally
            {
                conn.Close();
            }

            if (rowsAffected > 0)
                return true;

            return false;
        }



        //
        // MembershipProvider.GetAllUsers
        //

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand("SELECT Count(*) FROM `" + tableName + "` " +
                                              "WHERE ApplicationName = $ApplicationName", conn);
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = ApplicationName;

            MembershipUserCollection users = new MembershipUserCollection();

            SQLiteDataReader reader = null;
            totalRecords = 0;

            try
            {
                conn.Open();
                totalRecords = Convert.ToInt32(cmd.ExecuteScalar());

                if (totalRecords <= 0) { return users; }

                cmd.CommandText = "SELECT PKID, Username, Email, PasswordQuestion," +
                         " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," +
                         " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate " +
                         " FROM `" + tableName + "` " +
                         " WHERE ApplicationName = $ApplicationName " +
                         " ORDER BY Username Asc";

                reader = cmd.ExecuteReader();

                int counter = 0;
                int startIndex = pageSize * pageIndex;
                int endIndex = startIndex + pageSize - 1;

                while (reader.Read())
                {
                    if (counter >= startIndex)
                    {
                        MembershipUser u = GetUserFromReader(reader);
                        users.Add(u);
                    }

                    if (counter >= endIndex) { cmd.Cancel(); }

                    counter++;
                }
            }
            catch (SQLiteException e)
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
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }

            return users;
        }


        //
        // MembershipProvider.GetNumberOfUsersOnline
        //

        public override int GetNumberOfUsersOnline()
        {

            TimeSpan onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
            DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand("SELECT Count(*) FROM `" + tableName + "`" +
                    " WHERE LastActivityDate > $LastActivityDate AND ApplicationName = $ApplicationName", conn);

            cmd.Parameters.Add("$CompareDate", DbType.DateTime).Value = compareTime;
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;

            int numOnline = 0;

            try
            {
                conn.Open();

                numOnline = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (SQLiteException e)
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
            finally
            {
                conn.Close();
            }

            return numOnline;
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

            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand("SELECT Password, PasswordAnswer, IsLockedOut FROM `" + tableName + "`" +
                  " WHERE Username = $Username AND ApplicationName = $ApplicationName", conn);

            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username;
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;

            string password = "";
            string passwordAnswer = "";
            SQLiteDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.HasRows)
                {
                    reader.Read();

                    if (reader.GetBoolean(2))
                        throw new MembershipPasswordException("The supplied user is locked out.");

                    password = reader.GetString(0);
                    passwordAnswer = reader.GetString(1);
                }
                else
                {
                    throw new MembershipPasswordException("The supplied user name is not found.");
                }
            }
            catch (SQLiteException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetPassword");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }


            if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
            {
                UpdateFailureCount(username, "passwordAnswer");

                throw new MembershipPasswordException("Incorrect password answer.");
            }


            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                password = UnEncodePassword(password);
            }

            return password;
        }



        //
        // MembershipProvider.GetUser(string, bool)
        //

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand("SELECT PKID, Username, Email, PasswordQuestion," +
                 " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," +
                 " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate" +
                 " FROM `" + tableName + "` WHERE Username = $Username AND ApplicationName = $ApplicationName", conn);

            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username;
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;

            MembershipUser u = null;
            SQLiteDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    u = GetUserFromReader(reader);

                    if (userIsOnline)
                    {
                        SQLiteCommand updateCmd = new SQLiteCommand("UPDATE `" + tableName + "` " +
                                  "SET LastActivityDate = $LastActivityDate " +
                                  "WHERE Username = $Username AND ApplicationName = $ApplicationName", conn);

                        updateCmd.Parameters.Add("$LastActivityDate", DbType.DateTime).Value = DateTime.Now;
                        updateCmd.Parameters.Add("$Username", DbType.String, 255).Value = username;
                        updateCmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;

                        updateCmd.ExecuteNonQuery();
                    }
                }

            }
            catch (SQLiteException e)
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
            finally
            {
                if (reader != null) { reader.Close(); }

                conn.Close();
            }

            return u;
        }


        //
        // MembershipProvider.GetUser(object, bool)
        //

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand("SELECT PKID, Username, Email, PasswordQuestion," +
                  " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," +
                  " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate" +
                  " FROM `" + tableName + "` WHERE PKID = $PKID", conn);

            cmd.Parameters.Add("$PKID", DbType.String).Value = providerUserKey;

            MembershipUser u = null;
            SQLiteDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    u = GetUserFromReader(reader);

                    if (userIsOnline)
                    {
                        SQLiteCommand updateCmd = new SQLiteCommand("UPDATE `" + tableName + "` " +
                                  "SET LastActivityDate = $LastActivityDate " +
                                  "WHERE PKID = $PKID", conn);

                        updateCmd.Parameters.Add("$LastActivityDate", DbType.DateTime).Value = DateTime.Now;
                        updateCmd.Parameters.Add("$PKID", DbType.String).Value = providerUserKey;

                        updateCmd.ExecuteNonQuery();
                    }
                }

            }
            catch (SQLiteException e)
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
            finally
            {
                if (reader != null) { reader.Close(); }

                conn.Close();
            }

            return u;
        }


        //
        // GetUserFromReader
        //    A helper function that takes the current row from the SQLiteDataReader
        // and hydrates a MembershipUser from the values. Called by the 
        // MembershipUser.GetUser implementation.
        //

        private MembershipUser GetUserFromReader(SQLiteDataReader reader)
        {
            if (reader.GetString(1)=="") return null;
            object providerUserKey=null;
            string strGooid=Guid.NewGuid().ToString();
            if (reader.GetValue(0).ToString().Length > 0)
                providerUserKey = new Guid(reader.GetValue(0).ToString());
            else
                providerUserKey = new Guid(strGooid);
            string username = reader.GetString(1);
            string email = reader.GetString(2);

            string passwordQuestion = "";
            if (reader.GetValue(3) != DBNull.Value)
                passwordQuestion = reader.GetString(3);

            string comment = "";
            if (reader.GetValue(4) != DBNull.Value)
                comment = reader.GetString(4);

            bool tmpApproved = (reader.GetValue(5) == null);
             bool isApproved=false;
            if(tmpApproved)
            isApproved = reader.GetBoolean(5);

        bool tmpLockedOut = (reader.GetValue(6) == null);
        bool isLockedOut = false;
            if(tmpLockedOut)
            isLockedOut = reader.GetBoolean(6);

        DateTime creationDate = DateTime.Now;
            try
            {
                if (reader.GetValue(6) != DBNull.Value)
                    creationDate = reader.GetDateTime(7);
            }
            catch { }

            DateTime lastLoginDate = DateTime.Now;
            try
            {
                if (reader.GetValue(8) != DBNull.Value)
                    lastLoginDate = reader.GetDateTime(8);
            }
            catch { }

            DateTime lastActivityDate = DateTime.Now;
            try
            {
                if (reader.GetValue(9) != DBNull.Value)
                    lastActivityDate = reader.GetDateTime(9);
            }
            catch { }
            DateTime lastPasswordChangedDate = DateTime.Now;
            try
            {
                if (reader.GetValue(10) != DBNull.Value)
                    lastPasswordChangedDate = reader.GetDateTime(10);
            }
            catch { }

            DateTime lastLockedOutDate = DateTime.Now;
            try
            {
                if (reader.GetValue(11) != DBNull.Value)
                    lastLockedOutDate = reader.GetDateTime(11);
            }
            catch { }

            MembershipUser u = new MembershipUser(this.Name,
                                                  username,
                                                  providerUserKey,
                                                  email,
                                                  passwordQuestion,
                                                  comment,
                                                  isApproved,
                                                  isLockedOut,
                                                  creationDate,
                                                  lastLoginDate,
                                                  lastActivityDate,
                                                  lastPasswordChangedDate,
                                                  lastLockedOutDate);

            return u;
        }


        //
        // MembershipProvider.UnlockUser
        //

        public override bool UnlockUser(string username)
        {
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand("UPDATE `" + tableName + "` " +
                                              " SET IsLockedOut = False, LastLockedOutDate = $LastLockedOutDate " +
                                              " WHERE Username = $Username AND ApplicationName = $ApplicationName", conn);

            cmd.Parameters.Add("$LastLockedOutDate", DbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username;
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;

            int rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UnlockUser");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }

            if (rowsAffected > 0)
                return true;

            return false;
        }


        //
        // MembershipProvider.GetUserNameByEmail
        //

        public override string GetUserNameByEmail(string email)
        {
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand("SELECT Username" +
                  " FROM `" + tableName + "` WHERE Email = $Email AND ApplicationName = $ApplicationName", conn);

            cmd.Parameters.Add("$Email", DbType.String, 128).Value = email;
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;

            string username = "";

            try
            {
                conn.Open();
                object o = cmd.ExecuteScalar();
                if (o != null)   username = Convert.ToString(o);
            }
            catch (SQLiteException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUserNameByEmail");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }

            if (username == null)
                username = "";

            return username;
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

            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(username, "passwordAnswer");

                throw new ProviderException("Password answer required for password reset.");
            }

            string newPassword =
              System.Web.Security.Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);


            ValidatePasswordEventArgs args =
              new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");


            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand("SELECT PasswordAnswer, IsLockedOut FROM `" + tableName + "`" +
                  " WHERE Username = $Username AND ApplicationName = $ApplicationName", conn);

            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username;
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;

            int rowsAffected = 0;
            string passwordAnswer = "";
            SQLiteDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.HasRows)
                {
                    reader.Read();

                    //object val = reader.GetValue(1);

                    if (Convert.ToBoolean(reader.GetValue(1)))
                        throw new MembershipPasswordException("The supplied user is locked out.");

                    passwordAnswer = reader.GetString(0);
                }
                else
                {
                    throw new MembershipPasswordException("The supplied user name is not found.");
                }

                if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
                {
                    UpdateFailureCount(username, "passwordAnswer");

                    throw new MembershipPasswordException("Incorrect password answer.");
                }

                reader.Close();

                SQLiteCommand updateCmd = new SQLiteCommand("UPDATE `" + tableName + "`" +
                    " SET Password = $Password, LastPasswordChangedDate = $LastPasswordChangedDate" +
                    " WHERE Username = $Username AND ApplicationName = $ApplicationName AND IsLockedOut = 0", conn);

                updateCmd.Parameters.Add("$Password", DbType.String, 255).Value = EncodePassword(newPassword);
                updateCmd.Parameters.Add("$LastPasswordChangedDate", DbType.DateTime).Value = DateTime.Now;
                updateCmd.Parameters.Add("$Username", DbType.String, 255).Value = username;
                updateCmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;

                rowsAffected = updateCmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ResetPassword");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }

            if (rowsAffected > 0)
            {
                return newPassword;
            }
            else
            {
                throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.");
            }
        }


        //
        // MembershipProvider.UpdateUser
        //

        public override void UpdateUser(MembershipUser user)
        {
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand("UPDATE `" + tableName + "`" +
                    " SET Email = $Email, Comment = $Comment," +
                    " IsApproved = $IsApproved" +
                    " WHERE Username = $Username AND ApplicationName = $ApplicationName", conn);

            cmd.Parameters.Add("$Email", DbType.String, 128).Value = user.Email;
            cmd.Parameters.Add("$Comment", DbType.String, 255).Value = user.Comment;
            cmd.Parameters.Add("$IsApproved", DbType.Boolean).Value = user.IsApproved;
            cmd.Parameters.Add("$Username", DbType.String, 255).Value = user.UserName;
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;


            try
            {
                conn.Open();

                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UpdateUser");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
        }


        //
        // MembershipProvider.ValidateUser
        //

        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;

            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand("SELECT Password, IsApproved FROM `" + tableName + "`" +
                    " WHERE Username = $Username AND ApplicationName = $ApplicationName AND IsLockedOut = 0", conn);

            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username;
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;

            SQLiteDataReader reader = null;
            bool isApproved = false;
            string pwd = "";

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.HasRows)
                {
                    reader.Read();
                    pwd = reader.GetString(0);
                    int iApp = Convert.ToInt32(reader.GetValue(1));
                    if (iApp == 1) isApproved = true;
                }
                else
                {
                    return false;
                }

                reader.Close();

                if (CheckPassword(password, pwd))
                {
                    if (isApproved)
                    {
                        isValid = true;

                        SQLiteCommand updateCmd = new SQLiteCommand("UPDATE `" + tableName + "` SET LastLoginDate = $LastLoginDate" +
                                                                " WHERE Username = $Username AND ApplicationName = $ApplicationName", conn);

                        updateCmd.Parameters.Add("$LastLoginDate", DbType.DateTime).Value = DateTime.Now;
                        updateCmd.Parameters.Add("$Username", DbType.String, 255).Value = username;
                        updateCmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;

                        updateCmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    conn.Close();

                    UpdateFailureCount(username, "password");
                }
            }
            catch (SQLiteException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ValidateUser");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }

            return isValid;
        }


        //
        // UpdateFailureCount
        //   A helper method that performs the checks and updates associated with
        // password failure tracking.
        //

        private void UpdateFailureCount(string username, string failureType)
        {
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand("SELECT FailedPasswordAttemptCount, " +
                                              "  FailedPasswordAttemptWindowStart, " +
                                              "  FailedPasswordAnswerAttemptCount, " +
                                              "  FailedPasswordAnswerAttemptWindowStart " +
                                              "  FROM `" + tableName + "` " +
                                              "  WHERE Username = $Username AND ApplicationName = $ApplicationName", conn);

            cmd.Parameters.Add("$Username", DbType.String, 255).Value = username;
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;

            SQLiteDataReader reader = null;
            DateTime windowStart = new DateTime();
            int failureCount = 0;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.HasRows)
                {
                    reader.Read();

                    if (failureType == "password")
                    {
                        failureCount = reader.GetInt32(0);
                        try
                        {
                            windowStart = reader.GetDateTime(1);
                        }
                        catch
                        {
                            windowStart = DateTime.Now;
                        }
                    }

                    if (failureType == "passwordAnswer")
                    {
                        failureCount = reader.GetInt32(2);
                        windowStart = reader.GetDateTime(3);
                    }
                }

                reader.Close();

                DateTime windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

                if (failureCount == 0 || DateTime.Now > windowEnd)
                {
                    // First password failure or outside of PasswordAttemptWindow. 
                    // Start a new password failure count from 1 and a new window starting now.

                    if (failureType == "password")
                        cmd.CommandText = "UPDATE `" + tableName + "` " +
                                          "  SET FailedPasswordAttemptCount = $Count, " +
                                          "      FailedPasswordAttemptWindowStart = $WindowStart " +
                                          "  WHERE Username = $Username AND ApplicationName = $ApplicationName";

                    if (failureType == "passwordAnswer")
                        cmd.CommandText = "UPDATE `" + tableName + "` " +
                                          "  SET FailedPasswordAnswerAttemptCount = $Count, " +
                                          "      FailedPasswordAnswerAttemptWindowStart = $WindowStart " +
                                          "  WHERE Username = $Username AND ApplicationName = $ApplicationName";

                    cmd.Parameters.Clear();

                    cmd.Parameters.Add("$Count", DbType.Int32).Value = 1;
                    cmd.Parameters.Add("$WindowStart", DbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("$Username", DbType.String, 255).Value = username;
                    cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;

                    if (cmd.ExecuteNonQuery() < 0)
                        throw new ProviderException("Unable to update failure count and window start.");
                }
                else
                {
                    if (failureCount++ >= MaxInvalidPasswordAttempts)
                    {
                        // Password attempts have exceeded the failure threshold. Lock out
                        // the user.

                        cmd.CommandText = "UPDATE `" + tableName + "` " +
                                          "  SET IsLockedOut = $IsLockedOut, LastLockedOutDate = $LastLockedOutDate " +
                                          "  WHERE Username = $Username AND ApplicationName = $ApplicationName";

                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("$IsLockedOut", DbType.Boolean).Value = true;
                        cmd.Parameters.Add("$LastLockedOutDate", DbType.DateTime).Value = DateTime.Now;
                        cmd.Parameters.Add("$Username", DbType.String, 255).Value = username;
                        cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;

                        if (cmd.ExecuteNonQuery() < 0)
                            throw new ProviderException("Unable to lock out user.");
                    }
                    else
                    {
                        // Password attempts have not exceeded the failure threshold. Update
                        // the failure counts. Leave the window the same.

                        if (failureType == "password")
                            cmd.CommandText = "UPDATE `" + tableName + "` " +
                                              "  SET FailedPasswordAttemptCount = $Count" +
                                              "  WHERE Username = $Username AND ApplicationName = $ApplicationName";

                        if (failureType == "passwordAnswer")
                            cmd.CommandText = "UPDATE `" + tableName + "` " +
                                              "  SET FailedPasswordAnswerAttemptCount = $Count" +
                                              "  WHERE Username = $Username AND ApplicationName = $ApplicationName";

                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("$Count", DbType.Int32).Value = failureCount;
                        cmd.Parameters.Add("$Username", DbType.String, 255).Value = username;
                        cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;

                        if (cmd.ExecuteNonQuery() < 0)
                            throw new ProviderException("Unable to update failure count.");
                    }
                }
            }
            catch (SQLiteException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UpdateFailureCount");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }
        }


        //
        // CheckPassword
        //   Compares password values based on the MembershipPasswordFormat.
        //

        private bool CheckPassword(string password, string dbpassword)
        {
            string pass1 = password;
            string pass2 = dbpassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
                default:
                    break;
            }

            if (pass1 == pass2)
            {
                return true;
            }

            return false;
        }


        //
        // EncodePassword
        //   Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        //

        private string EncodePassword(string password)
        {
            if (password == null) password = "";
            string encodedPassword = password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword =
                      Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    HMACSHA1 hash = new HMACSHA1();
                    hash.Key = HexToByte(encryptionKey);
                    encodedPassword =
                      Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return encodedPassword;
        }


        //
        // UnEncodePassword
        //   Decrypts or leaves the password clear based on the PasswordFormat.
        //

        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                      Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot unencode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }

        //
        // HexToByte
        //   Converts a hexadecimal string to a byte array. Used to convert encryption
        // key values from the configuration.
        //

        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }


        //
        // MembershipProvider.FindUsersByName
        //

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {

            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand("SELECT Count(*) FROM `" + tableName + "` " +
                      "WHERE Username LIKE $UsernameSearch AND ApplicationName = $ApplicationName", conn);
            cmd.Parameters.Add("$UsernameSearch", DbType.String, 255).Value = usernameToMatch;
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = pApplicationName;

            MembershipUserCollection users = new MembershipUserCollection();

            SQLiteDataReader reader = null;

            try
            {
                conn.Open();
                totalRecords = Convert.ToInt32(cmd.ExecuteScalar());

                if (totalRecords <= 0) { return users; }

                cmd.CommandText = "SELECT PKID, Username, Email, PasswordQuestion," +
                  " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," +
                  " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate " +
                  " FROM `" + tableName + "` " +
                  " WHERE Username LIKE $UsernameSearch AND ApplicationName = $ApplicationName " +
                  " ORDER BY Username Asc";

                reader = cmd.ExecuteReader();

                int counter = 0;
                int startIndex = pageSize * pageIndex;
                int endIndex = startIndex + pageSize - 1;

                while (reader.Read())
                {
                    if (counter >= startIndex)
                    {
                        MembershipUser u = GetUserFromReader(reader);
                        users.Add(u);
                    }

                    if (counter >= endIndex) { cmd.Cancel(); }

                    counter++;
                }
            }
            catch (SQLiteException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "FindUsersByName");

                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }

                conn.Close();
            }

            return users;
        }

        //
        // MembershipProvider.FindUsersByEmail
        //

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            SQLiteCommand cmd = new SQLiteCommand("SELECT Count(*) FROM `" + tableName + "` " +
                                              "WHERE Email LIKE $EmailSearch AND ApplicationName = $ApplicationName", conn);
            cmd.Parameters.Add("$EmailSearch", DbType.String, 255).Value = emailToMatch;
            cmd.Parameters.Add("$ApplicationName", DbType.String, 255).Value = ApplicationName;

            MembershipUserCollection users = new MembershipUserCollection();

            SQLiteDataReader reader = null;
            totalRecords = 0;

            try
            {
                conn.Open();
                totalRecords = Convert.ToInt32(cmd.ExecuteScalar());

                if (totalRecords <= 0) { return users; }

                cmd.CommandText = "SELECT PKID, Username, Email, PasswordQuestion," +
                         " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," +
                         " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate " +
                         " FROM `" + tableName + "` " +
                         " WHERE Email LIKE $Username AND ApplicationName = $ApplicationName " +
                         " ORDER BY Username Asc";

                reader = cmd.ExecuteReader();

                int counter = 0;
                int startIndex = pageSize * pageIndex;
                int endIndex = startIndex + pageSize - 1;

                while (reader.Read())
                {
                    if (counter >= startIndex)
                    {
                        MembershipUser u = GetUserFromReader(reader);
                        users.Add(u);
                    }

                    if (counter >= endIndex) { cmd.Cancel(); }

                    counter++;
                }
            }
            catch (SQLiteException e)
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
            finally
            {
                if (reader != null) { reader.Close(); }

                conn.Close();
            }

            return users;
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