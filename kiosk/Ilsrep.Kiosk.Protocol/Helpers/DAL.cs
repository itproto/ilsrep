using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace Ilsrep.Kiosk.Protocol.Helpers
{
    public class DAL
    {
        /// <summary>
        /// Connection string specifying path to database file among other connection parameters
        /// </summary>
        static public string connectionString = "Data Source=\"App_Data/kiosk.db\"";
        /// <summary>
        /// Database connection
        /// </summary>
        static private SQLiteConnection dbConnection = null;
        /// <summary>
        /// Name of users table in database
        /// </summary>
        public const string USERS_TABLE = "users";
        /// <summary>
        /// Name of programs table in database
        /// </summary>
        public const string PROGRAMS_TABLE = "programs";

        /// <summary>
        /// Property that tells if database connection is active
        /// </summary>
        static public bool isConnected
        {
            get
            {
                return (dbConnection != null && dbConnection.State == System.Data.ConnectionState.Open);
            }
        }

        /// <summary>
        /// Close database connection
        /// </summary>
        static public void Close()
        {
            if (isConnected)
                dbConnection.Close();
        }

        /// <summary>
        /// Open database connection
        /// </summary>
        static private void Init()
        {
            dbConnection = new SQLiteConnection(connectionString);
            dbConnection.Open();
        }
    }
}
