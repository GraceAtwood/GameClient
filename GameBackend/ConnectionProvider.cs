using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace GameBackend
{
    public static class ConnectionProvider
    {
        public static string DatabaseLocation { get; set; } = null;

        public static SQLiteConnection GetConnection(string databaseLocation)
        {
            return new SQLiteConnection(string.Format("Data Source={0}", databaseLocation));
        }
    }
}
