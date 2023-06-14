using System;
using System.Data.SQLite;

namespace SimpleWindowsForm
{
    public class DatabaseManager
    {
        private SQLiteConnection connection;

        public DatabaseManager()
        {
            connection = new SQLiteConnection("Data Source=Task.db;Version=3;");
        }

        public void CreateTaskTable()
        {
            connection.Open();

            string createTableQuery = "CREATE TABLE IF NOT EXISTS Task (Id INTEGER PRIMARY KEY AUTOINCREMENT, TaskText TEXT, Urgency TEXT, DueDate TEXT)";
            SQLiteCommand command = new SQLiteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }

        public void CreateEmployeesTable()
        {
            connection.Open();

            string createTableQuery = "CREATE TABLE IF NOT EXISTS Employees (Id INTEGER PRIMARY KEY AUTOINCREMENT, Login TEXT, Name TEXT, Position TEXT)";
            SQLiteCommand command = new SQLiteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}
