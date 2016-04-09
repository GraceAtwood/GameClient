using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace GameBackend
{
    /// <summary>
    /// Provides members for interacting with dialogue.
    /// </summary>
    public static class DialogueProvider
    {
        private static readonly string _tableName = "DialogueLines";

        private static ConcurrentDictionary<int, string> _dialogueCache = new ConcurrentDictionary<int, string>();

        /// <summary>
        /// Asynchronously returns a line of dialogue for a given line ID. 
        /// <para/>
        /// Optionally uses the cache or loads freshly from the database.
        /// <para/>
        /// Throws an exception if a dialogue line isn't found.
        /// </summary>
        /// <param name="dialogueID">The line of dialogue to retrieve.</param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        public static async Task<string> FetchAsync(int dialogueID, bool useCache)
        {
            try
            {
                if (useCache)
                {
                    string value;
                    if (!_dialogueCache.TryGetValue(dialogueID, out value))
                        throw new Exception(string.Format("No dialogue line exists for the line ID '{0}'!", dialogueID));
                    return value;
                }

                using (SQLiteConnection connection = ConnectionProvider.GetConnection(ConnectionProvider.DatabaseLocation))
                {
                    await connection.OpenAsync();

                    using (SQLiteCommand command = new SQLiteCommand("", connection))
                    {
                        command.CommandText = string.Format("SELECT * FROM `{0}` WHERE `ID` = @ID", _tableName);

                        command.Parameters.AddWithValue("@ID", dialogueID);

                        using (SQLiteDataReader reader = (SQLiteDataReader)await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                await reader.ReadAsync();

                                return reader["Text"] as string;
                            }
                            else
                            {
                                throw new Exception(string.Format("No dialogue line exists for the line ID '{0}'!", dialogueID));
                            } 
                        }

                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Initializes the dialogue lines cache in order to expedite calls to this provider.
        /// </summary>
        /// <returns></returns>
        public static async Task InitializeCacheAsync()
        {
            try
            {
                _dialogueCache.Clear();

                using (SQLiteConnection connection = ConnectionProvider.GetConnection())
                {
                    await connection.OpenAsync();

                    using (SQLiteCommand command = new SQLiteCommand("", connection))
                    {
                        command.CommandText = string.Format("SELECT * FROM `{0}`", _tableName);

                        using (SQLiteDataReader reader = (SQLiteDataReader)await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    if (!_dialogueCache.TryAdd(reader.GetInt32(reader.GetOrdinal("ID")), reader["Text"] as string))
                                        throw new Exception("There was an error while attempting to load a line.");
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Inserts a new line into the database.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static async Task InsertAsync(string text)
        {
            try
            {
                using (SQLiteConnection connection = ConnectionProvider.GetConnection())
                {
                    await connection.OpenAsync();

                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {

                            using (SQLiteCommand command = new SQLiteCommand("", connection, transaction))
                            {
                                command.CommandText = string.Format("INSERT INTO `{0}` `TEXT` VALUES (@Text)", _tableName);

                                command.Parameters.AddWithValue("@Text", text);

                                await command.ExecuteNonQueryAsync();
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }

                    
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Updates a line of dialogue
        /// </summary>
        /// <param name="text"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task UpdateAsync(string text, int id)
        {
            try
            {
                using (SQLiteConnection connection = ConnectionProvider.GetConnection())
                {
                    await connection.OpenAsync();

                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {

                            using (SQLiteCommand command = new SQLiteCommand("", connection, transaction))
                            {
                                command.CommandText = string.Format("UPDATE `{0}` SET `Text` = @Text WHERE `ID` = @ID", _tableName);

                                command.Parameters.AddWithValue("@Text", text);
                                command.Parameters.AddWithValue("@ID", id);

                                if ((await command.ExecuteNonQueryAsync()) != 1)
                                    throw new Exception(string.Format("An error occurred while trying to update the dialogue line, '{0}'.  Perhaps it doesn't exist?", id));
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

            }
            catch
            {

                throw;
            }
        }

        /// <summary>
        /// Deletes a line of dialogue from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task DeleteAsync(int id)
        {
            try
            {
                using (SQLiteConnection connection = ConnectionProvider.GetConnection())
                {
                    await connection.OpenAsync();

                    using (SQLiteTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {

                            using (SQLiteCommand command = new SQLiteCommand("", connection, transaction))
                            {
                                command.CommandText = "DELETE FROM `{0}` WHERE `ID` = @ID";

                                command.Parameters.AddWithValue("@ID", id);

                                if ((await command.ExecuteNonQueryAsync()) != 1)
                                    throw new Exception(string.Format("An error occurred while trying to delete the dialogue line, '{0}'.  Perhaps it doesn't exist?", id));

                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }


    }
}
