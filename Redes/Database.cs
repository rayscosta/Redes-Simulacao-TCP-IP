using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace TCPIPSimulation
{
    class Database
    {
        private string connectionString;

        public Database(string server, string database, string user, string password)
        {
            connectionString = $"Server={server};Database={database};User ID={user};Password={password};";
        }

        public List<(string userName, int score, DateTime date)> GetAllScores()
        {
            var scores = new List<(string userName, int score, DateTime date)>();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT UserName, Score, Date FROM UserScores;";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string userName = reader.GetString(0);
                            int score = reader.GetInt32(1);
                            DateTime date = reader.GetDateTime(2);
                            scores.Add((userName, score, date));
                        }
                    }
                }
            }
            return scores;
        }

        public List<(string question, bool answer)> GetQuestions()
        {
            var questions = new List<(string question, bool answer)>();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Question, Answer FROM Questions ORDER BY RAND() LIMIT 5;";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string question = reader.GetString(0);
                            bool answer = reader.GetBoolean(1);
                            questions.Add((question, answer));
                        }
                    }
                }
            }
            return questions;
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