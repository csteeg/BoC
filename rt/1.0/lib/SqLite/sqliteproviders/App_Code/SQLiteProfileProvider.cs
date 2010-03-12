using System;
using System.Web;
using System.Web.Configuration;
using System.Web.Profile;
using System.Web.Security;
using System.Security.Principal;
using System.Security.Permissions;
using System.Globalization;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Text;
using System.Configuration.Provider;
using System.Configuration;
using System.Web.Hosting;
using System.Web.DataAccess;
using System.Web.Util;
using System.Diagnostics;
using System.Data.SQLite;


namespace Mascix.SQLiteProviders
{
    public sealed class SQLiteProfileProvider : ProfileProvider
    {
        private string _AppName;
        private string _connectionString;
        private int _ApplicationId = 0;
        private DateTime _ApplicationIDCacheDate;
        private string _Description;

        private object PKID;
        SQLiteCommand cmd = null;

        //Setup for the WriteToEventLog
        private string eventSource = "SQLiteProfileProvider";
        private string eventLog = "Application";
        private string exceptionMessage = "An exception occurred. Please check the Event Log.";
        private bool pWriteExceptionsToEventLog;
        public bool WriteExceptionsToEventLog
        {
            get { return pWriteExceptionsToEventLog; }
            set { pWriteExceptionsToEventLog = value; }
        }

        /////////////////////////////////////////////////////////////
        // Public properties                                       //
        //---------------------------------------------------------//
        public override void Initialize(string name, NameValueCollection config)
        {
            if (name == null || name.Length < 1)
                name = "MySQLProfileProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "$safeprojectname$ Profile Provider");
            }
            base.Initialize(name, config);
            if (config == null)
                throw new ArgumentNullException("config");

            ////////////////////////////////////////////////////////
            // Initialize SQLiteConnection.                        //
            //----------------------------------------------------//
            ConnectionStringSettings ConnectionStringSettings =
             ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

            if (ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim() == "")
            {
                throw new ProviderException("Connection String Empty");
            }
            _connectionString = ConnectionStringSettings.ConnectionString;
            ////////////////////////////////////////////////////////
            // Get the Application Name from Config               //
            //----------------------------------------------------//
            _AppName = config["applicationName"];
            if (string.IsNullOrEmpty(_AppName))
                _AppName = "";
            if (_AppName.Length > 255)
            {
                throw new ProviderException("ApplicationName exceededs " + 255);
            }
            ////////////////////////////////////////////////////////
            // Get the Description                                //
            //----------------------------------------------------//
            _Description = config["description"];

            ////////////////////////////////////////////////////////
            // Check for invalid parameters in the config         //
            //----------------------------------------------------//
            config.Remove("connectionStringName");
            config.Remove("applicationName");
            config.Remove("description");
            if (config.Count > 0)
            {
                string attribUnrecognized = config.GetKey(0);
                if (!String.IsNullOrEmpty(attribUnrecognized))
                    throw new ProviderException("Unrecognized attribute: " + attribUnrecognized);
            }
        }

        ////////////////////////////////////////////////////////
        // Override ApplicationName                           //
        //----------------------------------------------------//
        public override string ApplicationName
        {
            get { return _AppName; }
            set
            {
                if (value.Length > 255)
                    throw new ProviderException("ApplicationName exceeds " + 255);
                if (_AppName != value)
                {
                    _ApplicationId = 0;
                    _AppName = value;
                }
            }
        }

        ////////////////////////////////////////////////////////
        // Override GetPropertyValues                         //
        //----------------------------------------------------//
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext sc, SettingsPropertyCollection properties)
        {
            SettingsPropertyValueCollection svc = new SettingsPropertyValueCollection();
            if (properties.Count < 1)
                return svc;

            string username = (string)sc["UserName"];
            foreach (SettingsProperty prop in properties)
            {
                if (prop.SerializeAs == SettingsSerializeAs.ProviderSpecific)
                    if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string))
                        prop.SerializeAs = SettingsSerializeAs.String;
                    else
                        prop.SerializeAs = SettingsSerializeAs.Xml;
                svc.Add(new SettingsPropertyValue(prop));
            }

            if (!String.IsNullOrEmpty(username))
            {
                GetPropertyValuesFromDatabase(username, svc);
            }
            return svc;
        }
        ////////////////////////////////////////////////////////
        // Parse Data From Database                           //
        //----------------------------------------------------//
        private void ParseDataFromDB(string[] names, string values, byte[] buf, SettingsPropertyValueCollection properties)
        {
            if (names == null || values == null || buf == null || properties == null)
                return;
            try
            {
                for (int iter = 0; iter < names.Length / 4; iter++)
                {
                    string name = names[iter * 4];
                    SettingsPropertyValue pp = properties[name];

                    if (pp == null) // property not found
                        continue;

                    int startPos = Int32.Parse(names[iter * 4 + 2], CultureInfo.InvariantCulture);
                    int length = Int32.Parse(names[iter * 4 + 3], CultureInfo.InvariantCulture);

                    if (length == -1 && !pp.Property.PropertyType.IsValueType) // Null Value
                    {
                        pp.PropertyValue = null;
                        pp.IsDirty = false;
                        pp.Deserialized = true;
                    }
                    if (names[iter * 4 + 1] == "S" && startPos >= 0 && length > 0 && values.Length >= startPos + length)
                    {
                        pp.PropertyValue = Deserialize(pp, values.Substring(startPos, length));
                    }

                    if (names[iter * 4 + 1] == "B" && startPos >= 0 && length > 0 && buf.Length >= startPos + length)
                    {
                        byte[] buf2 = new byte[length];

                        Buffer.BlockCopy(buf, startPos, buf2, 0, length);
                        pp.PropertyValue = Deserialize(pp, buf2);
                    }
                }
            }
            catch
            {
            }
        }

        ////////////////////////////////////////////////////////
        // Get Property Values from Database                  //
        //----------------------------------------------------//
        private void GetPropertyValuesFromDatabase(string username, SettingsPropertyValueCollection svc)
        {
            try
            {
                SQLiteConnection holder = new SQLiteConnection(_connectionString);
                SQLiteDataReader reader = null;
                string[] names = null;
                string values = null;

                try
                {//read data
                    holder.Open();
                    int appId = GetApplicationId(holder);
                    cmd = new SQLiteCommand("SELECT PKID FROM Users WHERE UserName ='" + username + "'", holder);
                    PKID = cmd.ExecuteScalar();
                    if (PKID != null)
                    { // User exists?
                        cmd = new SQLiteCommand("SELECT PropertyNames, PropertyValuesString FROM aspnet_Profile WHERE PKID ='" + PKID + "'", holder);
                        reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            names = reader.GetString(0).Split(':');
                            values = reader.GetString(1);
                        }
                        /*try { // Not a critical part -- don't throw exceptions here
                            cmd = new SQLiteCommand("UPDATE users SET LastActivityDate='" + DateTime.Now.ToString("yyyy:MM:dd hh:mm:ss") + "' WHERE PKID ='" + PKID + "'", holder);
                            cmd.ExecuteNonQuery();
                        }
                        catch { }*/
                    }
                }
                catch (Exception e)
                {
                    if (WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(e, "Get Property From DB");
                        throw new ProviderException(exceptionMessage);
                    }
                    else
                    {
                        throw e;
                    }
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    holder.Close();
                }
                if (names != null && names.Length > 0)
                {
                    ParseDataFromDB(names, values, new byte[0], svc);
                }
            }
            catch
            {
                throw;
            }
        }

        ////////////////////////////////////////////////////////
        // Prep Data for Saving                               //
        //----------------------------------------------------//
        private static void PrepareDataForSaving(ref string allNames, ref string allValues, ref byte[] buf, bool binarySupported, SettingsPropertyValueCollection properties, bool userIsAuthenticated)
        {
            StringBuilder names = new StringBuilder();
            StringBuilder values = new StringBuilder();

            MemoryStream ms = (binarySupported ? new System.IO.MemoryStream() : null);
            try
            {
                try
                {
                    bool anyItemsToSave = false;

                    foreach (SettingsPropertyValue pp in properties)
                    {
                        if (pp.IsDirty)
                        {
                            if (!userIsAuthenticated)
                            {
                                bool allowAnonymous = (bool)pp.Property.Attributes["AllowAnonymous"];
                                if (!allowAnonymous)
                                    continue;
                            }
                            anyItemsToSave = true;
                            break;
                        }
                    }

                    if (!anyItemsToSave)
                        return;

                    foreach (SettingsPropertyValue pp in properties)
                    {
                        if (!userIsAuthenticated)
                        {
                            bool allowAnonymous = (bool)pp.Property.Attributes["AllowAnonymous"];
                            if (!allowAnonymous)
                                continue;
                        }

                        if (!pp.IsDirty && pp.UsingDefaultValue) // Not fetched from DB and not written to
                            continue;

                        int len = 0, startPos = 0;
                        string propValue = null;

                        if (pp.Deserialized && pp.PropertyValue == null)
                        { // is value null?
                            len = -1;
                        }
                        else
                        {
                            object sVal = SerializePropertyValue(pp);

                            if (sVal == null)
                            {
                                len = -1;
                            }
                            else
                            {
                                if (!(sVal is string) && !binarySupported)
                                {
                                    sVal = Convert.ToBase64String((byte[])sVal);
                                }

                                if (sVal is string)
                                {
                                    propValue = (string)sVal;
                                    len = propValue.Length;
                                    startPos = values.Length;
                                }
                                else
                                {
                                    byte[] b2 = (byte[])sVal;
                                    startPos = (int)ms.Position;
                                    ms.Write(b2, 0, b2.Length);
                                    ms.Position = startPos + b2.Length;
                                    len = b2.Length;
                                }
                            }
                        }

                        names.Append(pp.Name + ":" + ((propValue != null) ? "S" : "B") +
                                     ":" + startPos.ToString(CultureInfo.InvariantCulture) + ":" + len.ToString(CultureInfo.InvariantCulture) + ":");
                        if (propValue != null)
                            values.Append(propValue);
                    }

                    if (binarySupported)
                    {
                        buf = ms.ToArray();
                    }
                }
                finally
                {
                    if (ms != null)
                        ms.Close();
                }
            }
            catch
            {
                throw;
            }
            allNames = names.ToString();
            allValues = values.ToString();
        }

        ////////////////////////////////////////////////////////
        // Set Property Values                                //
        //----------------------------------------------------//
        public override void SetPropertyValues(SettingsContext sc, SettingsPropertyValueCollection properties)
        {

            try
            {
                string username = (string)sc["UserName"];
                bool userIsAuthenticated = (bool)sc["IsAuthenticated"];
                if (username == null || username.Length < 1 || properties.Count < 1)
                    return;
                string names = String.Empty;
                string values = String.Empty;
                byte[] buf = null;
                PrepareDataForSaving(ref names, ref values, ref buf, false, properties, userIsAuthenticated);
                if (names.Length == 0)
                    return;
                SQLiteConnection conn = new SQLiteConnection(_connectionString);
                SQLiteTransaction trans = null;
                bool fBeginTransCalled = false;
                try
                {//Store Data

                    conn.Open();
                    trans = conn.BeginTransaction();
                    fBeginTransCalled = true;

                    cmd = new SQLiteCommand("SELECT PKID FROM Users WHERE UserName = '" + username + "'", conn, trans);
                    PKID = cmd.ExecuteScalar();

                    cmd = new SQLiteCommand("SELECT PKID FROM aspnet_Profile WHERE PKID = '" + PKID + "'", conn, trans);
                    object result = cmd.ExecuteScalar();
                    string PKID1 = Convert.ToString(result);
                    string PKID2 = PKID.ToString();
                    if (result != null && (PKID1 == PKID2))
                    {
                        cmd = new SQLiteCommand("UPDATE aspnet_Profile SET PropertyNames ='" + names + "', PropertyValuesString ='" + values + "', LastUpdatedDate ='" + DateTime.Now.ToString("yyyy:MM:dd hh:mm:ss") + "' WHERE PKID ='" + PKID + "'", conn, trans);
                    }
                    else
                    {
                        cmd = new SQLiteCommand("INSERT INTO aspnet_Profile (PKID, PropertyNames, PropertyValuesString, LastUpdatedDate) VALUES ('" + PKID + "','" + names + "','" + values + "','" + DateTime.Now.ToString("yyyy:MM:dd hh:mm:ss") + "')", conn, trans);
                    }
                    cmd.ExecuteNonQuery();
                    try
                    { // Not a critical part -- don't throw exceptions here
                        cmd = new SQLiteCommand("UPDATE users SET LastActivityDate='" + DateTime.Now.ToString("yyyy:MM:dd hh:mm:ss") + "' WHERE PKID = '" + PKID + "'", conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch { }
                    trans.Commit();
                    fBeginTransCalled = false;
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    if (WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(e, "Error Setting Property");
                        throw new ProviderException(exceptionMessage);
                    }
                    else
                    {
                        throw e;
                    }
                }
                finally
                {
                    if (fBeginTransCalled)
                    {
                        try
                        {
                            trans.Rollback();
                        }
                        catch { }
                    }
                    conn.Close();
                }
            }
            catch
            {
                throw;
            }
        }

        ////////////////////////////////////////////////////////
        // Get Application Id                                 //
        //----------------------------------------------------//
        private int GetApplicationId(SQLiteConnection holder)
        {
            if (_ApplicationId != 0) // Already cached?
                return _ApplicationId;
            string appName = _AppName;
            if (appName.Length > 255)
                appName = appName.Substring(0, 255);
            try
            {
                cmd = new SQLiteCommand("SELECT ApplicationId FROM aspnet_applications WHERE ApplicationName = '" + appName + "'", holder);
                _ApplicationId = (Convert.ToInt32(cmd.ExecuteScalar()));

                _ApplicationIDCacheDate = DateTime.Now;
                if (_ApplicationId >= 0)
                    return _ApplicationId;
                throw new ProviderException("Could not get ApplicationId");
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "Error Getting AppId");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }

        }

        ////////////////////////////////////////////////////////
        // Mangement APIs from ProfileProvider class          //
        //----------------------------------------------------//

        ////////////////////////////////////////////////////////
        // Delete Profiles                                    //
        //----------------------------------------------------//
        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            SQLiteConnection holder = new SQLiteConnection(_connectionString);
            SQLiteTransaction trans = null;

            if (profiles == null)
            {
                throw new ArgumentNullException("profiles");
            }

            if (profiles.Count < 1)
            {
                throw new ArgumentException("Profiles collection is empty", "profiles");
            }

            foreach (ProfileInfo pi in profiles)
            {
                string username = pi.UserName;
                //SecUtility.CheckParameter(ref username, true, true, true, 255, "UserName");
            }

            try
            {

                bool fBeginTransCalled = false;
                int numDeleted = 0;
                try
                {
                    holder.Open();
                    trans = holder.BeginTransaction();
                    fBeginTransCalled = true;
                    int appId = GetApplicationId(holder);
                    foreach (ProfileInfo profile in profiles)
                        if (DeleteProfile(holder, profile.UserName.Trim(), appId))
                            numDeleted++;
                    trans.Commit();
                    fBeginTransCalled = false;
                }
                catch (Exception e)
                {
                    if (WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(e, "Delete Profile");

                        throw new ProviderException(exceptionMessage);
                    }
                    else
                    {
                        throw e;
                    }
                }
                finally
                {
                    if (fBeginTransCalled)
                    {
                        try
                        {
                            trans.Rollback();
                        }
                        catch { }
                    }
                    holder.Close();
                }
                return numDeleted;
            }
            catch
            {
                throw;
            }
        }

        ////////////////////////////////////////////////////////
        // Delete Profiles                                    //
        //----------------------------------------------------//
        public override int DeleteProfiles(string[] usernames)
        {
            //SecUtility.CheckArrayParameter(ref usernames, true,true,true,255,"usernames");
            SQLiteConnection holder = new SQLiteConnection(_connectionString);
            SQLiteTransaction trans = null;
            try
            {

                int numDeleted = 0;
                bool fBeginTransCalled = false;
                try
                {
                    holder.Open();
                    trans = holder.BeginTransaction();
                    fBeginTransCalled = true;
                    int appId = GetApplicationId(holder);
                    foreach (string username in usernames)
                        if (DeleteProfile(holder, username, appId))
                            numDeleted++;
                    trans.Commit();
                    fBeginTransCalled = false;
                }
                catch (Exception e)
                {
                    if (WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(e, "Delete Profiles");

                        throw new ProviderException(exceptionMessage);
                    }
                    else
                    {
                        throw e;
                    }
                }
                finally
                {
                    if (fBeginTransCalled)
                    {
                        try
                        {
                            trans.Rollback();
                        }
                        catch { }
                    }
                    holder.Close();
                }
                return numDeleted;
            }
            catch
            {
                throw;
            }
        }

        ////////////////////////////////////////////////////////
        // Delete Inactive Profiles                           //
        //----------------------------------------------------//
        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            int appId = GetApplicationId(new SQLiteConnection(_connectionString));
            string inClause = "SELECT PKID FROM users WHERE ApplicationId ='" + appId + "' AND LastActivityDate <= '" + userInactiveSinceDate.ToString("yyyy:MM:dd hh:mm:ss") + "' " + GetClauseForAuthenticationOptions(authenticationOption);
            string sqlQuery = "DELETE FROM aspnet_Profile WHERE PKID IN (" + inClause + ")";
            SQLiteConnection conn = new SQLiteConnection(_connectionString);
            int Result;
            try
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(sqlQuery, conn);
                Result = cmd.ExecuteNonQuery();
                return Result;
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "Delete Inactive Profiles");

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

        ////////////////////////////////////////////////////////
        // Get Number of Inactive Profiles                    //
        //----------------------------------------------------//
        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            int appId = GetApplicationId(new SQLiteConnection(_connectionString));
            string sqlQuery = "SELECT COUNT(*) FROM users u, aspnet_Profile p " +
              "WHERE ApplicationId ='" + appId + "' AND LastActivityDate <= '" + userInactiveSinceDate.ToString("yyyy:MM:dd hh:mm:ss") + "' AND u.PKID = p.PKID" + GetClauseForAuthenticationOptions(authenticationOption);
            SQLiteConnection conn = new SQLiteConnection(_connectionString);
            int Result;
            try
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(sqlQuery, conn);
                Result = cmd.ExecuteNonQuery();
                return Result;
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "Delete Inactive Profiles");

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

        ////////////////////////////////////////////////////////
        // Get All Profiles                                   //
        //----------------------------------------------------//
        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            int appId = GetApplicationId(new SQLiteConnection(_connectionString));
            string sqlQuery = "SELECT u.UserName, u.IsAnonymous, u.LastActivityDate, p.LastUpdatedDate, LEN(p.PropertyNames) + LEN(p.PropertyValuesString) FROM users u, aspnet_Profile p WHERE ApplicationId ='" + appId + "' AND u.PKID = p.PKID " + GetClauseForAuthenticationOptions(authenticationOption);
            SQLiteParameter[] args = new SQLiteParameter[0];
            return GetProfilesForQuery(sqlQuery, args, pageIndex, pageSize, out totalRecords);
        }

        ////////////////////////////////////////////////////////
        // Get All Inactive Profiles                          //
        //----------------------------------------------------//
        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            int appId = GetApplicationId(new SQLiteConnection(_connectionString));
            string sqlQuery = "SELECT u.UserName, u.IsAnonymous, u.LastActivityDate, p.LastUpdatedDate, LEN(p.PropertyNames) + LEN(p.PropertyValuesString) FROM users u, aspnet_Profile p WHERE ApplicationId ='" + appId + "' AND u.PKID = p.PKID AND u.LastActivityDate <= '" + userInactiveSinceDate.ToString("yyyy:MM:dd hh:mm:ss") + "' " + GetClauseForAuthenticationOptions(authenticationOption);
            SQLiteParameter[] args = new SQLiteParameter[0];
            return GetProfilesForQuery(sqlQuery, args, pageIndex, pageSize, out totalRecords);
        }
        ////////////////////////////////////////////////////////
        // Find Profiles By Username                          //
        //----------------------------------------------------//
        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            //SecUtility.CheckParameter(ref usernameToMatch, true, true, false, 255, "username");
            int appId = GetApplicationId(new SQLiteConnection(_connectionString));
            string sqlQuery = "SELECT u.UserName, u.IsAnonymous, u.LastActivityDate, p.LastUpdatedDate, LEN(p.PropertyNames) + LEN(p.PropertyValuesString) FROM users u, aspnet_Profile p WHERE ApplicationId ='" + appId + "' AND u.PKID = p.PKID AND u.UserName LIKE '" + usernameToMatch + "'" + GetClauseForAuthenticationOptions(authenticationOption);
            SQLiteParameter[] args = new SQLiteParameter[0];
            return GetProfilesForQuery(sqlQuery, args, pageIndex, pageSize, out totalRecords);
        }

        ////////////////////////////////////////////////////////
        // Find Inactive Profiles By UserName                 //
        //----------------------------------------------------//
        public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            //SecUtility.CheckParameter(ref usernameToMatch, true, true, false, 255, "usernameToMatch");
            int appId = GetApplicationId(new SQLiteConnection(_connectionString));
            string sqlQuery = "SELECT u.UserName, u.IsAnonymous, u.LastActivityDate, p.LastUpdatedDate, LEN(p.PropertyNames) + LEN(p.PropertyValuesString) FROM users u, aspnet_Profile p WHERE ApplicationId ='" + appId + "' AND u.PKID = p.PKID AND u.UserName like '" + usernameToMatch + "' AND u.LastActivityDate <='" + userInactiveSinceDate.ToString("yyyy:MM:dd hh:mm:ss") + "'" + GetClauseForAuthenticationOptions(authenticationOption);
            SQLiteParameter[] args = new SQLiteParameter[0];
            return GetProfilesForQuery(sqlQuery, args, pageIndex, pageSize, out totalRecords);
        }

        ////////////////////////////////////////////////////////
        // Private Methods                                    //
        //----------------------------------------------------//

        ////////////////////////////////////////////////////////
        // Get Profiles for Query                             //
        //----------------------------------------------------//
        private ProfileInfoCollection GetProfilesForQuery(string sqlQuery, SQLiteParameter[] args, int pageIndex, int pageSize, out int totalRecords)
        {
            if (pageIndex < 0)
                throw new ArgumentException("Page index must be non-negative", "pageIndex");
            if (pageSize < 1)
                throw new ArgumentException("Page size must be positive", "pageSize");

            long lBound = (long)pageIndex * pageSize;
            long uBound = lBound + pageSize - 1;

            if (uBound > System.Int32.MaxValue)
            {
                throw new ArgumentException("pageIndex*pageSize too large");
            }
            SQLiteConnection holder = new SQLiteConnection(_connectionString);
            ProfileInfoCollection profiles = new ProfileInfoCollection();
            SQLiteDataReader reader = null;
            holder.Open();
            SQLiteCommand cmd = new SQLiteCommand(sqlQuery, holder);
            int len = args.Length;
            for (int iter = 0; iter < len; iter++)
                reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
            totalRecords = 0;
            while (reader.Read())
            {
                totalRecords++;
                if (totalRecords - 1 < lBound || totalRecords - 1 > uBound)
                    continue;

                string username;
                DateTime dtLastActivity, dtLastUpdated;
                bool isAnon;

                username = reader.GetString(0);
                isAnon = reader.GetBoolean(1);
                dtLastActivity = reader.GetDateTime(2);
                dtLastUpdated = reader.GetDateTime(3);
                int size = reader.GetInt32(4);
                profiles.Add(new ProfileInfo(username, isAnon, dtLastActivity, dtLastUpdated, size));


            }
            holder.Close();
            return profiles;
        }

        ////////////////////////////////////////////////////////
        // Delete Profile                                     //
        //----------------------------------------------------//
        private bool DeleteProfile(SQLiteConnection holder, string username, int appId)
        {
            //SecUtility.CheckParameter(ref username, true, true, true, 255, "username");
            holder.Open();
            cmd = new SQLiteCommand("SELECT PKID FROM Users WHERE UserName ='" + username + "'", holder);
            PKID = cmd.ExecuteScalar();
            if (PKID.ToString() == "0")
                return false;
            cmd = new SQLiteCommand("DELETE FROM aspnet_Profile WHERE PKID ='" + PKID + "'", holder);
            bool Result;
            Result = cmd.ExecuteNonQuery() != 0;
            holder.Close();
            return (Result);
        }
        ////////////////////////////////////////////////////////
        // Get Clause For Authentication Options              //
        //----------------------------------------------------//
        static private string GetClauseForAuthenticationOptions(ProfileAuthenticationOption authenticationOption)
        {
            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    return " AND IsAnonymous=Yes ";

                case ProfileAuthenticationOption.Authenticated:
                    return " AND IsAnonymous=No ";

                case ProfileAuthenticationOption.All:
                    return " ";
            }
            return " ";
        }

        ////////////////////////////////////////////////////////
        // Convert Object to String                           //
        //----------------------------------------------------//
        private static string ConvertObjectToString(object propValue, Type type, SettingsSerializeAs serializeAs, bool throwOnError)
        {
            if (serializeAs == SettingsSerializeAs.ProviderSpecific)
            {
                if (type == typeof(string) || type.IsPrimitive)
                    serializeAs = SettingsSerializeAs.String;
                else
                    serializeAs = SettingsSerializeAs.Xml;
            }

            try
            {
                switch (serializeAs)
                {
                    case SettingsSerializeAs.String:
                        TypeConverter converter = TypeDescriptor.GetConverter(type);
                        if (converter != null && converter.CanConvertTo(typeof(String)) && converter.CanConvertFrom(typeof(String)))
                            return converter.ConvertToString(propValue);
                        throw new ArgumentException("Unable to convert type " + type.ToString() + " to string", "type");
                    case SettingsSerializeAs.Binary:
                        MemoryStream ms = new System.IO.MemoryStream();
                        try
                        {
                            BinaryFormatter bf = new BinaryFormatter();
                            bf.Serialize(ms, propValue);
                            byte[] buffer = ms.ToArray();
                            return Convert.ToBase64String(buffer);
                        }
                        finally
                        {
                            ms.Close();
                        }

                    case SettingsSerializeAs.Xml:
                        XmlSerializer xs = new XmlSerializer(type);
                        StringWriter sw = new StringWriter(CultureInfo.InvariantCulture);

                        xs.Serialize(sw, propValue);
                        return sw.ToString();
                }
            }
            catch (Exception)
            {
                if (throwOnError)
                    throw;
            }
            return null;
        }

        ////////////////////////////////////////////////////////
        // Serialize Property Value                           //
        //----------------------------------------------------//
        private static object SerializePropertyValue(SettingsPropertyValue prop)
        {
            object val = prop.PropertyValue;
            if (val == null)
                return null;

            if (prop.Property.SerializeAs != SettingsSerializeAs.Binary)
                return ConvertObjectToString(val, prop.Property.PropertyType, prop.Property.SerializeAs, prop.Property.ThrowOnErrorSerializing);

            MemoryStream ms = new System.IO.MemoryStream();
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, val);
                return ms.ToArray();
            }
            finally
            {
                ms.Close();
            }
        }
        ////////////////////////////////////////////////////////
        // Deserialize Property Value                         //
        //----------------------------------------------------//
        private object Deserialize(SettingsPropertyValue prop, object obj)
        {
            object val = null;

            //////////////////////////////////////////////
            /// Step 1: Try creating from Serailized value
            if (obj != null)
            {
                try
                {
                    if (obj is string)
                    {
                        val = GetObjectFromString(prop.Property.PropertyType, prop.Property.SerializeAs, (string)obj);
                    }
                    else
                    {
                        MemoryStream ms = new System.IO.MemoryStream((byte[])obj);
                        try
                        {
                            val = (new BinaryFormatter()).Deserialize(ms);
                        }
                        finally
                        {
                            ms.Close();
                        }
                    }
                }
                catch
                {
                }

                if (val != null && !prop.Property.PropertyType.IsAssignableFrom(val.GetType())) // is it the correct type
                    val = null;
            }

            //////////////////////////////////////////////
            /// Step 2: Try creating from default value
            if (val == null)
            {
                if (prop.Property.DefaultValue == null || prop.Property.DefaultValue.ToString() == "[null]")
                {
                    if (prop.Property.PropertyType.IsValueType)
                        return Activator.CreateInstance(prop.Property.PropertyType);
                    else
                        return null;
                }
                if (!(prop.Property.DefaultValue is string))
                {
                    val = prop.Property.DefaultValue;
                }
                else
                {
                    try
                    {
                        val = GetObjectFromString(prop.Property.PropertyType, prop.Property.SerializeAs, (string)prop.Property.DefaultValue);
                    }
                    catch (Exception e)
                    {
                        if (WriteExceptionsToEventLog)
                        {
                            WriteToEventLog(e, "Create from Default");

                            throw new ProviderException(exceptionMessage);
                        }
                        else
                        {
                            throw e;
                        }
                    }
                }
                if (val != null && !prop.Property.PropertyType.IsAssignableFrom(val.GetType())) // is it the correct type
                    throw new ArgumentException("Could not create from default value for property: " + prop.Property.Name);
            }

            //////////////////////////////////////////////
            /// Step 3: Create a new one by calling the parameterless constructor
            if (val == null)
            {
                if (prop.Property.PropertyType == typeof(string))
                {
                    val = "";
                }
                else
                {
                    try
                    {
                        val = Activator.CreateInstance(prop.Property.PropertyType);
                    }
                    catch { }
                }
            }
            return val;
        }

        ////////////////////////////////////////////////////////
        // Get Object From String                             //
        //----------------------------------------------------//
        private static object GetObjectFromString(Type type, SettingsSerializeAs serializeAs, string attValue)
        {
            // Deal with string types
            if (type == typeof(string) && (attValue == null || attValue.Length < 1 || serializeAs == SettingsSerializeAs.String))
                return attValue;

            // Return null if there is nothing to convert
            if (attValue == null || attValue.Length < 1)
                return null;

            // Convert based on the serialized type
            switch (serializeAs)
            {
                case SettingsSerializeAs.Binary:
                    byte[] buf = Convert.FromBase64String(attValue);
                    MemoryStream ms = null;
                    try
                    {
                        ms = new System.IO.MemoryStream(buf);
                        return (new BinaryFormatter()).Deserialize(ms);
                    }
                    finally
                    {
                        if (ms != null)
                            ms.Close();
                    }

                case SettingsSerializeAs.Xml:
                    StringReader sr = new StringReader(attValue);
                    XmlSerializer xs = new XmlSerializer(type);
                    return xs.Deserialize(sr);

                case SettingsSerializeAs.String:
                    TypeConverter converter = TypeDescriptor.GetConverter(type);
                    if (converter != null && converter.CanConvertTo(typeof(String)) && converter.CanConvertFrom(typeof(String)))
                        return converter.ConvertFromString(attValue);
                    throw new ArgumentException("Unable to convert type: " + type.ToString() + " from string", "type");

                default:
                    return null;
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

        ////////////////////////////////////////////////////////
        // Write To Event Log                                 //
        //----------------------------------------------------//
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
