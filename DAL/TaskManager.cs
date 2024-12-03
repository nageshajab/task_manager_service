using TaskManager.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using Task = TaskManager.Models.Task;

namespace DAL
{
    public class TaskManager
    {
        public string ConnectionString = Environment.GetEnvironmentVariable("ApplicationDbContext");

        public List<Task> ListTasksByUserId(TaskSearch TaskSearch)
        {
            List<Task> Tasks = [];

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                SqlCommand command = new();
                command.Connection = connection;
                command.CommandText = $"select * from Tasks where userid={TaskSearch.UserId}";
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Task Task = new()
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        Description = reader.GetString("description"),
                        DueDate= reader.GetDateTime("duedate"),
                        Priority= (Priority)Enum.Parse(typeof(Priority), reader.GetString("priority")),
                        Status= (Status)Enum.Parse(typeof(Status), reader.GetString("status")),
                        UserId = reader.GetInt32("userid"),                        
                        CanRepeat =Boolean.Parse( reader.GetString("repeat")),
                        EndDate= reader.GetDateTime("enddate"),
                        RepeatType= (RepeatType)Enum.Parse( typeof(RepeatType), reader.GetString("repeatType"))
                    };

                    Tasks.Add(Task);
                }

                connection.Close();
            }
            return Tasks;
        }

        public int Insert(Task Task)
        {
            int Taskid;
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"INSERT INTO [dbo].[Task] ([title],[description],[duedate],[priority],[status],[userid],[Repeat] ,[enddate],[repeatType]) VALUES ('{Task.Title}','{Task.Description}','{Task.DueDate}','{Task.Priority}','{Task.Status}',{Task.UserId},{Task.CanRepeat},'{Task.EndDate}','{Task.RepeatType}')";

                    command.Transaction = transaction;
                    Taskid = (int)command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return Taskid;
        }

        public bool DeleteAllTasks()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = "delete Task";
                    command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }

        public string Update(Task Task, int id)
        {
            Task TaskFromDb = Get(id);

            if (TaskFromDb.Id == 0)
            {
                return "Task with id not found";
            }

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = $"UPDATE [dbo].[Task] SET [title] = '{Task.Title}' ,[description] = '{Task.Description}' ,[duedate] ='{Task.DueDate}',[priority] = '{Task.Priority}' ,[status] = '{Task.Status}',[userid] = {Task.UserId}, [Repeat] = {Task.CanRepeat}, [enddate] = '{Task.EndDate}', [repeatType] ='{Task.RepeatType}' WHERE id={id}";

                    command.Transaction = transaction;
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
            return "updated";
        }

        public Task Get(int id)
        {
            Task Task = new();

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = $"select * from Task where id={id}";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Task = new()
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        Description = reader.GetString("description"),
                        DueDate = reader.GetDateTime("duedate"),
                        Priority = (Priority)Enum.Parse(typeof(Priority), reader.GetString("priority")),
                        Status = (Status)Enum.Parse(typeof(Status), reader.GetString("status")),
                        UserId = reader.GetInt32("userid"),
                        CanRepeat = Boolean.Parse(reader.GetString("repeat")),
                        EndDate = reader.GetDateTime("enddate"),
                        RepeatType = (RepeatType)Enum.Parse(typeof(RepeatType), reader.GetString("repeatType"))
                    };
                }
                connection.Close();
            }
            return Task;
        }

        public bool DeleteTask(int id)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.Transaction = transaction;
                    command.CommandText = "delete Task where id=" + id;
                    command.ExecuteScalar();

                    transaction.Commit();
                }
            }
            return true;
        }
    }
}
