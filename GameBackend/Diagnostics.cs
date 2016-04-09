using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace GameBackend
{
    /// <summary>
    /// Provides methods for interacting with the backend to determine its health and other connection stats.
    /// </summary>
    public static class Diagnostics
    {
        /// <summary>
        /// Tests the connection to the database given a database location.
        /// <para/>
        /// Returns true or false if the connection is established.
        /// </summary>
        /// <param name="databaseLocation"></param>
        /// <returns></returns>
        public static async Task<bool> TestDatabaseConnectionAsync(string databaseLocation)
        {
            try
            {
                using (SQLiteConnection connection = ConnectionProvider.GetConnection(databaseLocation))
                {
                    await connection.OpenAsync();
                }

                //If we get here, then the connection opened successfully!
                return true; 
            }
            catch
            {
                return false;
            }
        }
    }
}
