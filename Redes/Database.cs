using MySql.Data.MySqlClient;
using System;

namespace TCPIPSimulation
{
    class Database
    {
        private string connectionString;

        public Database(string server, string database, string user, string password)
        {
            connectionString = $"Server={server};Database={database};User ID={user};Password={password};";
        }

        public void CreateTable()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS UserScores (
                        Id INT AUTO_INCREMENT PRIMARY KEY,
                        UserName VARCHAR(100) NOT NULL,
                        Score INT NOT NULL,
                        Date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    );";
                using (var command = new MySqlCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void InsertScore(string userName, int score)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO UserScores (UserName, Score) VALUES (@UserName, @Score);";
                using (var command = new MySqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@Score", score);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}