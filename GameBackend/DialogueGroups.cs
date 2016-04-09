using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Utilities.SerializationExtensionMethods;

namespace GameBackend
{
    /// <summary>
    /// Provides members for interacting with dialogue groups.
    /// </summary>
    public static class DialogueGroups
    {
        /// <summary>
        /// Standardizes access to the dialogue lines table in the database.
        /// </summary>
        private static readonly string _tableName = "DialogueGroups";

        /// <summary>
        /// A static concurrent cache used to manage multi-thread access to the dialogue groups.
        /// </summary>
        private static ConcurrentDictionary<string, DialogueGroup> _dialogueGroupsCache = new ConcurrentDictionary<string, DialogueGroup>();

        /// <summary>
        /// Describes a single dialogue group and its data access methods.
        /// </summary>
        public class DialogueGroup
        {
            #region Properties

            /// <summary>
            /// The ID assigned to this group of dialogue elements.  Uniqueness isn't enforced, so it must be handled at the data access layer.
            /// </summary>
            public string ID { get; set; } = null;

            /// <summary>
            /// The elements of text in this group. 
            /// </summary>
            public List<string> Elements { get; set; } = new List<string>();

            #endregion

            #region Data Access Methods

            /// <summary>
            /// Inserts this dialogue group into the database and optionally refreshes the cache.
            /// </summary>
            /// <param name="refreshCache"></param>
            /// <returns></returns>
            public async Task DBInsert(bool refreshCache)
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
                                    command.CommandText = string.Format("INSERT INTO `{0}` `ID`,`Elements` VALUES (@ID,@Elements)", _tableName);

                                    command.Parameters.AddWithValue("@ID", this.ID);
                                    command.Parameters.AddWithValue("@Elements", this.Elements.Serialize());

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

                    if (refreshCache)
                        await InitializeCacheAsync();
                }
                catch
                {
                    throw;
                }
            }

            /// <summary>
            /// Updates this object by using the ID.
            /// </summary>
            /// <param name="refreshCache"></param>
            /// <returns></returns>
            public async Task DBUpdate(bool refreshCache)
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
                                    command.CommandText = string.Format("UPDATE `{0}` SET `Elements` = @Elements WHERE `ID` = @ID", _tableName);

                                    command.Parameters.AddWithValue("@Elements", this.Elements.Serialize());

                                    command.Parameters.AddWithValue("@ID", this.ID);

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

                    if (refreshCache)
                        await InitializeCacheAsync();
                }
                catch
                {

                    throw;
                }
            }

            /// <summary>
            /// Deletes this object by using the ID.
            /// </summary>
            /// <param name="refreshCache"></param>
            /// <returns></returns>
            public async Task DBDelete(bool refreshCache)
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
                                    command.CommandText = string.Format("DELETE FROM `{0}` WHERE `ID` = @ID", _tableName);

                                    command.Parameters.AddWithValue("@ID", this.ID);

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

                    if (refreshCache)
                        await InitializeCacheAsync();
                }
                catch
                {
                    throw;
                }
            }

            #endregion

        }

        /// <summary>
        /// Asynchronously loads a dialogue group for a given ID from the database or throws an error if no line is found.
        /// </summary>
        /// <param name="groupID">The ID of the group to load.</param>
        /// <returns></returns>
        public static async Task<DialogueGroup> DBFetch(string groupID)
        {
            try
            {
                using (SQLiteConnection connection = ConnectionProvider.GetConnection(ConnectionProvider.DatabaseLocation))
                {
                    await connection.OpenAsync();

                    using (SQLiteCommand command = new SQLiteCommand("", connection))
                    {
                        command.CommandText = string.Format("SELECT * FROM `{0}` WHERE `ID` = @ID", _tableName);

                        command.Parameters.AddWithValue("@ID", groupID);

                        using (SQLiteDataReader reader = (SQLiteDataReader)await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                await reader.ReadAsync();

                                return new DialogueGroup
                                {
                                    Elements = (reader["Elements"] as string).Deserialize<List<string>>(),
                                    ID = reader["ID"] as string
                                };
                            }
                            else
                            {
                                throw new Exception(string.Format("No dialogue group exists for the group ID '{0}'!", groupID));
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
        /// Fetches a dialogue group from the cache.
        /// </summary>
        /// <param name="groupID">The ID of the group to fetch.</param>
        /// <returns></returns>
        public static DialogueGroup Fetch(string groupID)
        {
            try
            {
                DialogueGroup group;
                if (!_dialogueGroupsCache.TryGetValue(groupID, out group))
                    throw new Exception(string.Format("The dialogue group ID was found in the for the ID, '{0}'.", groupID));
                return group;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Initializes the dialogue groups cache by clearing it and then loading all elements from the database into it.
        /// </summary>
        /// <returns></returns>
        public static async Task InitializeCacheAsync()
        {
            try
            {
                _dialogueGroupsCache.Clear();

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

                                    DialogueGroup group = new DialogueGroup
                                    {
                                        Elements = (reader["Elements"] as string).Deserialize<List<string>>(),
                                        ID = reader["ID"] as string
                                    };

                                    if (!_dialogueGroupsCache.TryAdd(group.ID, group))
                                        throw new Exception("There was an error while attempting to load a group into the cache.");
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

    }
}
