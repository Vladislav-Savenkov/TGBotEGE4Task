using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mono.Data.Sqlite;

namespace TelegramBotEGERus4
{
    public class DataManager
    {
        private string dbName = "URI=file:TelegramBotEGERus4.db";
        public async void CreateDB()
        {
            using (var connection = new SqliteConnection(dbName))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS Users (UserId INTEGER PRIMARY KEY NOT NULL, UserTaskId INTEGER NOT NULL);";
                    await command.ExecuteNonQueryAsync();
                }

                await connection.CloseAsync();
            }
        }

        public async Task<User> FindUserAsync(long UserId)
        {
            User user = new User();
            using (var connection = new SqliteConnection(dbName))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT * FROM Users WHERE UserId = {UserId}";
                    using (var data = await command.ExecuteReaderAsync())
                    {
                        user.UserId = UserId;
                        if (data.Read())
                        {
                            user.UserTaskId = data.GetInt64(1);
                        }
                        else
                        {
                            await AddUserAsync(UserId);
                            user.UserTaskId = -1;
                        }
                    }

                }
                await connection.CloseAsync();
            }
            return user;
        }

        private async Task AddUserAsync(long UserId)
        {
            using (var connection = new SqliteConnection(dbName))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO Users (UserId, UserTaskId) VALUES ('{UserId}', '-1');";
                    await command.ExecuteNonQueryAsync();
                }

                await connection.CloseAsync();
            }
        }

        public async Task UpdateUserTaskAsync(long UserId, long UserTaskId)
        {
            using (var connection = new SqliteConnection(dbName))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE Users SET UserTaskId = '{UserTaskId}' WHERE UserId = {UserId};";
                    await command.ExecuteNonQueryAsync();
                }

                await connection.CloseAsync();
            }
        }
    }
}
