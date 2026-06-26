using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace InnocentGuardPart3
{
    
    class TaskManager
    {
        private List<TaskItem> tasks = new List<TaskItem>();
        private string connectionString = "server=localhost;port=3306;database=innocentguard;uid=root;password=root_@223;";
        private int nextId = 1;

        public TaskManager()
        {
            TryCreateDatabase();
            LoadFromDatabase();
        }

        
        private void TryCreateDatabase()
        {
            try
            {
                string setupConn = "server=localhost;port=3306;uid=root;password=root_@223;";
                using (var conn = new MySqlConnection(setupConn))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "CREATE DATABASE IF NOT EXISTS innocentguard;" +
                        "USE innocentguard;" +
                        "CREATE TABLE IF NOT EXISTS tasks (" +
                        "id INT PRIMARY KEY AUTO_INCREMENT," +
                        "title VARCHAR(255) NOT NULL," +
                        "description VARCHAR(500)," +
                        "reminder VARCHAR(100)," +
                        "is_completed TINYINT DEFAULT 0);", conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                //MySQL might not be installed — we fall back to in-memory only
            }
        }

        
        private void LoadFromDatabase()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT * FROM tasks", conn);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var task = new TaskItem(
                            reader.GetInt32("id"),
                            reader.GetString("title"),
                            reader.GetString("description"),
                            reader.GetString("reminder")
                        );
                        task.IsCompleted = reader.GetInt32("is_completed") == 1;
                        tasks.Add(task);
                        if (task.Id >= nextId) nextId = task.Id + 1;
                    }
                }
            }
            catch
            {
                //No database
            }
        }

        //Add a new task
        public TaskItem AddTask(string title, string description, string reminder = "")
        {
            var task = new TaskItem(nextId++, title, description, reminder);
            tasks.Add(task);

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "INSERT INTO tasks (title, description, reminder, is_completed) VALUES (@t, @d, @r, 0)", conn);
                    cmd.Parameters.AddWithValue("@t", title);
                    cmd.Parameters.AddWithValue("@d", description);
                    cmd.Parameters.AddWithValue("@r", reminder);
                    cmd.ExecuteNonQuery();
                    task.Id = (int)cmd.LastInsertedId;
                }
            }
            catch { /* save in memory only if DB unavailable */ }

            return task;
        }

        //Mark a task as completed
        public bool CompleteTask(int id)
        {
            var task = tasks.Find(t => t.Id == id);
            if (task == null) return false;

            task.IsCompleted = true;

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("UPDATE tasks SET is_completed=1 WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }

            return true;
        }

        //Delete a task
        public bool DeleteTask(int id)
        {
            var task = tasks.Find(t => t.Id == id);
            if (task == null) return false;

            tasks.Remove(task);

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("DELETE FROM tasks WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }

            return true;
        }
        public string GetAllTasks()
        {
            if (tasks.Count == 0)
                return "You have no tasks yet. Type 'add task' to add one!";

            string result = "Here are your cybersecurity tasks:\n\n";
            foreach (var task in tasks)
                result += $"  {task}\n";

            return result;
        }

        public List<TaskItem> GetTasks() => tasks;
    }
}