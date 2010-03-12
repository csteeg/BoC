using System;
using System.Configuration.Provider;
using System.Security.Permissions;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Configuration;

namespace Mascix.SQLiteProviders
{
    public class SQLitePersonalizationProvider : PersonalizationProvider
    {
        private string connectionString;
        private string m_ApplicationName;
        public override string ApplicationName
        {
            get { return m_ApplicationName; }
            set { m_ApplicationName = value; }
        }

        private string m_ConnectionStringName;

        public string ConnectionStringName
        {
            get { return m_ConnectionStringName; }
            set { m_ConnectionStringName = value; }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            // Verify that config isn't null
            if (config == null)
                throw new ArgumentNullException("config");

            // Assign the provider a default name if it doesn't have one
            if (String.IsNullOrEmpty(name))
                name = "SQLitePersonalizationProvider";

            // Add a default "description" attribute to config if the
            // attribute doesn't exist or is empty
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description",
                    "Simple SQLite personalization provider");
            }

            // Call the base class's Initialize method
            base.Initialize(name, config);

            if (string.IsNullOrEmpty(config["connectionStringName"]))
            {
                throw new ProviderException
                    ("ConnectionStringName property has not been specified");
            }
            else
            {
                m_ConnectionStringName = config["connectionStringName"];
                config.Remove("connectionStringName");
            }

            if (string.IsNullOrEmpty(config["applicationName"]))
            {
                throw new ProviderException
                    ("applicationName property has not been specified");
            }
            else
            {
                m_ApplicationName = config["applicationName"];
                config.Remove("applicationName");
            }

            //
            // Initialize SQLiteConnection.
            //

            ConnectionStringSettings ConnectionStringSettings =
              ConfigurationManager.ConnectionStrings[m_ConnectionStringName];

            if (ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim() == "")
            {
                throw new ProviderException("Connection string cannot be blank.");
            }

            connectionString = ConnectionStringSettings.ConnectionString;

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException
                        ("Unrecognized attribute: " + attr);
            }

        }

        protected override void LoadPersonalizationBlobs
            (WebPartManager webPartManager, string path, string userName,
            ref byte[] sharedDataBlob, ref byte[] userDataBlob)
        {
            // Load shared state
            sharedDataBlob = null;
            userDataBlob = null;
            object sharedBlobDataObject = null;
            object userBlobDataObject = null;
            string sSQLShared = null;
            string sSQLUser = null;
            SQLiteConnection conn = new SQLiteConnection(connectionString);

            try
            {
                conn.Open();
                sSQLUser = "SELECT `personalizationblob` FROM `personalization`" + Environment.NewLine +
                    "WHERE `username` = '" + userName + "' AND " + Environment.NewLine +
                    "`path` = '" + path + "' AND " + Environment.NewLine +
                    "`applicationname` = '" + m_ApplicationName + "';";
                sSQLShared = "SELECT `personalizationblob` FROM `personalization`" + Environment.NewLine +
                    "WHERE `username` IS NULL AND " + Environment.NewLine +
                    "`path` = '" + path + "' AND " + Environment.NewLine +
                    "`applicationname` = '" + m_ApplicationName + "';";
                SQLiteCommand cmd = new SQLiteCommand(sSQLUser, conn);
                sharedBlobDataObject = cmd.ExecuteScalar();
                cmd.CommandText = sSQLShared;
                userBlobDataObject = cmd.ExecuteScalar();
                if (sharedBlobDataObject != null)
                    sharedDataBlob = (byte[])sharedBlobDataObject;
                if (userBlobDataObject != null)
                    userDataBlob = (byte[])userBlobDataObject;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        protected override void ResetPersonalizationBlob
            (WebPartManager webPartManager, string path, string userName)
        {
            // Delete the specified personalization file
            string sSQL = null;
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            try
            {
                sSQL = "DELETE FROM `personalization` WHERE `username` = '" + userName + "' AND `path` = '" + path + "' AND `applicationname` = '" + m_ApplicationName + "';";
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(sSQL, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception) { throw; }
            finally
            {
                conn.Close();
            }
        }

        protected override void SavePersonalizationBlob
            (WebPartManager webPartManager, string path, string userName,
            byte[] dataBlob)
        {
            string sSQL = null;
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            try
            {
                conn.Open();
                sSQL = "SELECT COUNT(`username`) FROM `personalization` WHERE `username` = '" + userName + "' AND `path` = '" + path + "' and `applicationname` = '" + m_ApplicationName + "';";
                SQLiteCommand cmd = new SQLiteCommand(sSQL, conn);
                if (int.Parse(cmd.ExecuteScalar().ToString()) > 0)
                {
                    sSQL = @"UPDATE `personalization` SET `personalizationblob` = $personalizationblob
                    WHERE `username` = $username AND 
                    `applicationname` = $applicationname AND 
                    `path` = $path;";
                    cmd = new SQLiteCommand(sSQL, conn);
                    cmd.Parameters.AddWithValue("$personalizationblob", dataBlob);
                    cmd.Parameters.AddWithValue("$username", userName);
                    cmd.Parameters.AddWithValue("$applicationname", m_ApplicationName);
                    cmd.Parameters.AddWithValue("$path", path);
                }
                else
                {
                    sSQL = @"INSERT INTO `personalization` (`username`,`path`,`applicationname`,`personalizationblob`) 
                            VALUES ($username,$path,$applicationname,$personalizationblob);";
                    cmd = new SQLiteCommand(sSQL, conn);
                    cmd.Parameters.AddWithValue("$username", userName);
                    cmd.Parameters.AddWithValue("$path", path);
                    cmd.Parameters.AddWithValue("$applicationname", m_ApplicationName);
                    cmd.Parameters.AddWithValue("$personalizationblob", dataBlob);
                }
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        public override PersonalizationStateInfoCollection FindState
            (PersonalizationScope scope, PersonalizationStateQuery query,
            int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        public override int GetCountOfState(PersonalizationScope scope,
            PersonalizationStateQuery query)
        {
            throw new NotSupportedException();
        }

        public override int ResetState(PersonalizationScope scope,
            string[] paths, string[] usernames)
        {
            throw new NotSupportedException();
        }

        public override int ResetUserState(string path,
            DateTime userInactiveSinceDate)
        {
            throw new NotSupportedException();
        }
    }

}
