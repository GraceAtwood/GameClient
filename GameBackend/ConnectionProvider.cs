using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace GameBackend
{
    /// <summary>
    /// Provides connections from the SQLite connection pool.
    /// </summary>
    internal static class ConnectionProvider
    {
        /// <summary>
        /// The location of the application's database.
        /// </summary>
        internal static string DatabaseLocation { get; set; } = null;

        /// <summary>
        /// Creates a new SQLite connection given a database location.
        /// </summary>
        /// <param name="databaseLocation"></param>
        /// <returns></returns>
        internal static SQLiteConnection GetConnection(string databaseLocation)
        {
            return new SQLiteConnection(string.Format("Data Source={0}", databaseLocation));
        }

        /// <summary>
        /// Creates a new SQLite connection and assumes the DatabaseLocation as the location.
        /// </summary>
        /// <returns></returns>
        internal static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(string.Format("Data Source={0}", DatabaseLocation));
        }
    }
}
